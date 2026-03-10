namespace MoneyBee.Shared.Core.Exceptions
{
    public static class ErrorCodes
    {
        public const string InternalServerError = "internal_server_error";
        public const string ValidationError = "validation_error";
        public const string InputRequired = "input_required";

        public const string SenderCustomerIdRequired = "sender_customer_id_required";
        public const string ReceiverCustomerIdRequired = "receiver_customer_id_required";
        public const string EmployeeIdRequired = "employee_id_required";
        public const string AmountMustBePositive = "amount_must_be_positive";
        public const string TransactionCodeRequired = "transaction_code_required";
        public const string IdempotencyKeyRequired = "idempotency_key_required";

        public const string TransferNotFound = "transfer_not_found";
        public const string SenderCustomerNotFound = "sender_customer_not_found";
        public const string ReceiverCustomerNotFound = "receiver_customer_not_found";
        public const string CustomerNotFound = "customer_not_found";

        public const string DailyLimitExceeded = "daily_limit_exceeded";
        public const string ReceiverCustomerMismatch = "receiver_customer_mismatch";
        public const string TransferAlreadyCompleted = "transfer_already_completed";
        public const string TransferAlreadyCancelled = "transfer_already_cancelled";
        public const string CompletedTransferCannotBeCancelled = "completed_transfer_cannot_be_cancelled";
        public const string CancelledTransferCannotBeCompleted = "cancelled_transfer_cannot_be_completed";

        public const string ConcurrencyConflict = "concurrency_conflict";
        public const string IdempotencyConflict = "idempotency_conflict";
        public const string IdempotencyKeyPayloadMismatch = "idempotency_key_payload_mismatch";
    }
}