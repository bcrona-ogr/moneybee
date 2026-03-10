using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using MoneyBee.Shared.API.Response;
using MoneyBee.IntegrationTests.Infrastructure;
using MoneyBee.Transfer.Contracts.Requests.Cancel;
using MoneyBee.Transfer.Contracts.Requests.Complete;
using MoneyBee.Transfer.Contracts.Requests.Create;
using MoneyBee.Transfer.Contracts.Responses;
using MoneyBee.Transfer.Contracts.Responses.History;

namespace MoneyBee.IntegrationTests.Transfer
{
    [Collection("postgres")]
    public class TransferApiTests : IDisposable
    {
        private readonly TransferApiFactory _factory;
        private readonly HttpClient _client;
        private readonly static Guid SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        private readonly static Guid ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        private readonly static Guid SenderNotFoundCustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private readonly static Guid ReceiverNotFoundCustomerId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public TransferApiTests(PostgresContainerFixture postgres)
        {
            _factory = new TransferApiFactory(postgres.TransferConnectionString);
            _client = _factory.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _factory.Dispose();
        }


        [Fact]
        public async Task Create_Should_Return401_When_TokenMissing()
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "/api/transfers")
            {
                Content = JsonContent.Create(new CreateTransferHttpRequest
                {
                    SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    Amount = 500m
                })
            };

            message.Headers.Add("Idempotency-Key", "create-unauthorized-1");

            var response = await _client.SendAsync(message);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Create_Should_Return200_When_RequestValid()
        {
            var response = await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = 500m
            }, "create-valid-1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<TransferHttpResponse>();

            body.Should().NotBeNull();
            body.Id.Should().NotBe(Guid.Empty);
            body.TransactionCode.Should().StartWith("TRX-");
            body.Amount.Should().Be(500m);
            body.Fee.Should().Be(20m);
            body.Currency.Should().Be(949);
            body.Status.Should().Be("ReadyForPickup");
            body.SenderCustomerId.Should().Be(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
            body.ReceiverCustomerId.Should().Be(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));
        }

        [Fact]
        public async Task Create_Should_Return404_When_SenderNotFound()
        {
            var response = await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = SenderNotFoundCustomerId,
                ReceiverCustomerId = ReceiverCustomerId,
                Amount = 500m
            }, "create-sender-notfound-1");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var body = await response.Content.ReadFromJsonAsync<ErrorHttpResponse>();

            body.Should().NotBeNull();
            body.Message.Should().Be("Sender customer not found.");
        }

        [Fact]
        public async Task Create_Should_ReturnSameResponse_When_SameIdempotencyKey_And_SamePayload_AreSentAgain()
        {
            var request = new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = 500m
            };

            var response1 = await SendCreateTransferAsync(request, "idem-int-1");
            response1.StatusCode.Should().Be(HttpStatusCode.OK);

            var body1 = await response1.Content.ReadFromJsonAsync<TransferHttpResponse>();
            body1.Should().NotBeNull();

            var response2 = await SendCreateTransferAsync(request, "idem-int-1");
            response2.StatusCode.Should().Be(HttpStatusCode.OK);

            var body2 = await response2.Content.ReadFromJsonAsync<TransferHttpResponse>();
            body2.Should().NotBeNull();

            body2.Id.Should().Be(body1.Id);
            body2.TransactionCode.Should().Be(body1.TransactionCode);
            body2.Amount.Should().Be(body1.Amount);
            body2.Fee.Should().Be(body1.Fee);
            body2.Currency.Should().Be(body1.Currency);
            body2.Status.Should().Be(body1.Status);
        }

        [Fact]
        public async Task Create_Should_Return409_When_SameIdempotencyKey_IsUsedWith_DifferentPayload()
        {
            var request1 = new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = 500m
            };

            var response1 = await SendCreateTransferAsync(request1, "idem-int-2");
            response1.StatusCode.Should().Be(HttpStatusCode.OK);

            var request2 = new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = 700m
            };

            var response2 = await SendCreateTransferAsync(request2, "idem-int-2");
            response2.StatusCode.Should().Be(HttpStatusCode.Conflict);

            var body = await response2.Content.ReadFromJsonAsync<ErrorHttpResponse>();
            body.Should().NotBeNull();
            body.Message.Should().Be("Idempotency key was already used with a different request.");
        }
        [Fact]
        public async Task Create_Should_Return404_When_ReceiverNotFound()
        {
            var response = await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = SenderCustomerId,
                ReceiverCustomerId = ReceiverNotFoundCustomerId,
                Amount = 500m
            }, "create-receiver-notfound-1");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var body = await response.Content.ReadFromJsonAsync<ErrorHttpResponse>();

            body.Should().NotBeNull();
            body.Message.Should().Be("Receiver customer not found.");
        }
        [Fact]
        public async Task Create_Should_Return422_When_RequestInvalid()
        {
            var response = await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Empty,
                ReceiverCustomerId = Guid.Empty,
                Amount = 0m
            }, "create-invalid-1");

            response.StatusCode.Should().Be((HttpStatusCode) 422);

            var body = await response.Content.ReadFromJsonAsync<ErrorHttpResponse>();

            body.Should().NotBeNull();
            body.StatusCode.Should().Be(422);
        }

        [Fact]
        public async Task Create_Should_Return400_When_DailyLimitExceeded()
        {
            var response = await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = 150000m
            }, "create-limit-1");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await response.Content.ReadFromJsonAsync<ErrorHttpResponse>();

            body.Should().NotBeNull();
            body.Message.Should().Be("Daily transfer limit exceeded.");
        }

        [Fact]
        public async Task GetByCode_Should_Return200_When_TransferExists()
        {
            var createResponse = await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = 500m
            }, "getbycode-create-1");

            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var created = await createResponse.Content.ReadFromJsonAsync<TransferHttpResponse>();
            created.Should().NotBeNull();

            var response = await SendAuthorizedAsync(HttpMethod.Get, $"/api/transfers/by-code/{created.TransactionCode}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<TransferHttpResponse>();
            body.Should().NotBeNull();
            body.TransactionCode.Should().Be(created.TransactionCode);
        }

        [Fact]
        public async Task Complete_Should_Return200_When_TransferExists_And_ReceiverMatches()
        {
            var createResponse = await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = 250m
            }, "complete-create-1");

            var created = await createResponse.Content.ReadFromJsonAsync<TransferHttpResponse>();
            created.Should().NotBeNull();

            var response = await SendAuthorizedAsync(HttpMethod.Post, "/api/transfers/complete", new CompleteTransferHttpRequest
            {
                TransactionCode = created.TransactionCode,
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var raw = await response.Content.ReadAsStringAsync();
            raw.Should().Contain("Completed");
        }

        [Fact]
        public async Task Complete_Should_Return400_When_ReceiverCustomerDoesNotMatch()
        {
            var createResponse = await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = 250m
            }, "complete-create-2");

            var created = await createResponse.Content.ReadFromJsonAsync<TransferHttpResponse>();
            created.Should().NotBeNull();

            var response = await SendAuthorizedAsync(HttpMethod.Post, "/api/transfers/complete", new CompleteTransferHttpRequest
            {
                TransactionCode = created.TransactionCode,
                ReceiverCustomerId = Guid.NewGuid()
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await response.Content.ReadFromJsonAsync<ErrorHttpResponse>();

            body.Should().NotBeNull();
            body.Message.Should().Be("Receiver customer does not match transfer.");
        }

        [Fact]
        public async Task Cancel_Should_Return200_When_TransferExists()
        {
            var createResponse = await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = 150m
            }, "cancel-create-1");

            var created = await createResponse.Content.ReadFromJsonAsync<TransferHttpResponse>();
            created.Should().NotBeNull();

            var response = await SendAuthorizedAsync(HttpMethod.Post, "/api/transfers/cancel", new CancelTransferHttpRequest
            {
                TransactionCode = created.TransactionCode
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var raw = await response.Content.ReadAsStringAsync();
            raw.Should().Contain("Cancelled");
        }

        [Fact]
        public async Task History_Should_Return200_When_TransfersExist()
        {
            await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = 100m
            }, "history-create-1");

            await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = 200m
            }, "history-create-2");

            var response = await SendAuthorizedAsync(HttpMethod.Get, "/api/transfers/history?customerId=aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa&role=sender");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<TransferHistoryHttpResponse>();

            body.Should().NotBeNull();
            body.Items.Count.Should().BeGreaterThanOrEqualTo(2);
        }

        private async Task<HttpResponseMessage> SendCreateTransferAsync(CreateTransferHttpRequest request, string idempotencyKey)
        {
            var token = TestJwtTokenFactory.CreateToken();

            var message = new HttpRequestMessage(HttpMethod.Post, "/api/transfers")
            {
                Content = JsonContent.Create(request)
            };

            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            message.Headers.Add("Idempotency-Key", idempotencyKey);

            return await _client.SendAsync(message);
        }

        private async Task<HttpResponseMessage> SendAuthorizedAsync(HttpMethod method, string url, object body = null)
        {
            var token = TestJwtTokenFactory.CreateToken();

            var message = new HttpRequestMessage(method, url);
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (body != null)
                message.Content = JsonContent.Create(body);

            return await _client.SendAsync(message);
        }
        
        [Fact]
        public async Task Create_Should_Return400_When_SenderAndReceiverAreSame()
        {
            var sameCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

            var response = await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = sameCustomerId,
                ReceiverCustomerId = sameCustomerId,
                Amount = 500m
            }, "create-same-sender-receiver-1");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await response.Content.ReadFromJsonAsync<ErrorHttpResponse>();
            body.Should().NotBeNull();
        }
        
        [Fact]
        public async Task Complete_Should_Return404_When_TransactionCodeNotFound()
        {
            var response = await SendAuthorizedAsync(HttpMethod.Post, "/api/transfers/complete", new CompleteTransferHttpRequest
            {
                TransactionCode = "TRX-NOT-FOUND",
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")
            });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var body = await response.Content.ReadFromJsonAsync<ErrorHttpResponse>();
            body.Should().NotBeNull();
        }
        
        [Fact]
        public async Task Cancel_Should_Return404_When_TransactionCodeNotFound()
        {
            var response = await SendAuthorizedAsync(HttpMethod.Post, "/api/transfers/cancel", new CancelTransferHttpRequest
            {
                TransactionCode = "TRX-NOT-FOUND"
            });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var body = await response.Content.ReadFromJsonAsync<ErrorHttpResponse>();
            body.Should().NotBeNull();
        }
        
        [Fact]
        public async Task Complete_Should_Return400_When_TransferAlreadyCompleted()
        {
            var createResponse = await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = 250m
            }, "complete-already-completed-create-1");

            var created = await createResponse.Content.ReadFromJsonAsync<TransferHttpResponse>();
            created.Should().NotBeNull();

            var firstComplete = await SendAuthorizedAsync(HttpMethod.Post, "/api/transfers/complete", new CompleteTransferHttpRequest
            {
                TransactionCode = created.TransactionCode,
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")
            });

            firstComplete.StatusCode.Should().Be(HttpStatusCode.OK);

            var secondComplete = await SendAuthorizedAsync(HttpMethod.Post, "/api/transfers/complete", new CompleteTransferHttpRequest
            {
                TransactionCode = created.TransactionCode,
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")
            });

            secondComplete.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await secondComplete.Content.ReadFromJsonAsync<ErrorHttpResponse>();
            body.Should().NotBeNull();
        }
        
        [Fact]
        public async Task Complete_Should_Return400_When_TransferAlreadyCancelled()
        {
            var createResponse = await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = 250m
            }, "complete-cancelled-create-1");

            var created = await createResponse.Content.ReadFromJsonAsync<TransferHttpResponse>();
            created.Should().NotBeNull();

            var cancelResponse = await SendAuthorizedAsync(HttpMethod.Post, "/api/transfers/cancel", new CancelTransferHttpRequest
            {
                TransactionCode = created.TransactionCode
            });

            cancelResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var completeResponse = await SendAuthorizedAsync(HttpMethod.Post, "/api/transfers/complete", new CompleteTransferHttpRequest
            {
                TransactionCode = created.TransactionCode,
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")
            });

            completeResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await completeResponse.Content.ReadFromJsonAsync<ErrorHttpResponse>();
            body.Should().NotBeNull();
        }
        [Fact]
        public async Task Cancel_Should_Return400_When_TransferAlreadyCompleted()
        {
            var createResponse = await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = 250m
            }, "cancel-completed-create-1");

            var created = await createResponse.Content.ReadFromJsonAsync<TransferHttpResponse>();
            created.Should().NotBeNull();

            var completeResponse = await SendAuthorizedAsync(HttpMethod.Post, "/api/transfers/complete", new CompleteTransferHttpRequest
            {
                TransactionCode = created.TransactionCode,
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")
            });

            completeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var cancelResponse = await SendAuthorizedAsync(HttpMethod.Post, "/api/transfers/cancel", new CancelTransferHttpRequest
            {
                TransactionCode = created.TransactionCode
            });

            cancelResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await cancelResponse.Content.ReadFromJsonAsync<ErrorHttpResponse>();
            body.Should().NotBeNull();
        }
        
        [Fact]
        public async Task Cancel_Should_Return400_When_TransferAlreadyCancelled()
        {
            var createResponse = await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = 250m
            }, "cancel-already-cancelled-create-1");

            var created = await createResponse.Content.ReadFromJsonAsync<TransferHttpResponse>();
            created.Should().NotBeNull();

            var firstCancel = await SendAuthorizedAsync(HttpMethod.Post, "/api/transfers/cancel", new CancelTransferHttpRequest
            {
                TransactionCode = created.TransactionCode
            });

            firstCancel.StatusCode.Should().Be(HttpStatusCode.OK);

            var secondCancel = await SendAuthorizedAsync(HttpMethod.Post, "/api/transfers/cancel", new CancelTransferHttpRequest
            {
                TransactionCode = created.TransactionCode
            });

            secondCancel.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var body = await secondCancel.Content.ReadFromJsonAsync<ErrorHttpResponse>();
            body.Should().NotBeNull();
        }
        
        [Fact]
        public async Task History_Should_Return200_WithEmptyItems_When_NoTransfersExist()
        {
            var emptySenderId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

            var response = await SendAuthorizedAsync(
                HttpMethod.Get,
                $"/api/transfers/history?customerId={emptySenderId}&role=sender");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<TransferHistoryHttpResponse>();
            body.Should().NotBeNull();
            body.Items.Should().NotBeNull();
            body.Items.Should().BeEmpty();
        }
        
        [Fact]
        public async Task Create_Should_Return400_When_AmountIsNegative()
        {
            var response = await SendCreateTransferAsync(new CreateTransferHttpRequest
            {
                SenderCustomerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                ReceiverCustomerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Amount = -10m
            }, "create-negative-amount-1");

            response.StatusCode.Should().Be((HttpStatusCode) 422);
        }
    }
}