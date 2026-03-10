using MoneyBee.Shared.Core.Entities;
using MoneyBee.Shared.Core.Exceptions;

namespace MoneyBee.Transfer.Domain.Entities
{
    public  class IdempotencyRecord : BaseEntity
    {
        public Guid Id { get; private set; }
        public string IdempotencyKey { get; private set; }
        public string OperationName { get; private set; }
        public Guid ActorId { get; private set; }
        public string RequestHash { get; private set; }
        public string ResponsePayload { get; private set; }
        public int ResponseStatusCode { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime ExpiresAtUtc { get; private set; }

        private IdempotencyRecord()
        {
        }

        public IdempotencyRecord(Guid id, string idempotencyKey, string operationName, Guid actorId, string requestHash, string responsePayload, int responseStatusCode, DateTime createdAtUtc, DateTime expiresAtUtc)
        {
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                throw new BusinessException("Idempotency key is required.");

            if (string.IsNullOrWhiteSpace(operationName))
                throw new BusinessException("Operation name is required.");

            if (actorId == Guid.Empty)
                throw new BusinessException("Actor id is required.");

            if (string.IsNullOrWhiteSpace(requestHash))
                throw new BusinessException("Request hash is required.");

            Id = id;
            IdempotencyKey = idempotencyKey.Trim();
            OperationName = operationName.Trim();
            ActorId = actorId;
            RequestHash = requestHash.Trim();
            ResponsePayload = responsePayload;
            ResponseStatusCode = responseStatusCode;
            CreatedAtUtc = createdAtUtc;
            ExpiresAtUtc = expiresAtUtc;
        }
    }
}