using System.Collections.Generic;

namespace Dnata.FlightBuilder.App.Shared
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<ValidationResultMessage> Messages { get; set; }
        public object Result { get; set; }

        public ValidationResult()
        {
            IsValid = true;
            Messages = new List<ValidationResultMessage>();
        }


        public ValidationResult Invalidate(string message)
        {
            IsValid = false;

            Messages.Add(new ValidationResultMessage { MessageText = message });

            return this;
        }

        public ValidationResult Invalidate(string code, string message)
        {
            IsValid = false;
            Messages.Add(new ValidationResultMessage { Code = code, MessageText = message });

            return this;
        }

        public ValidationResult Ingest(ValidationResult validationResult)
        {
            if (!validationResult.IsValid)
                IsValid = false;

            foreach (var message in validationResult.Messages)
                Messages.Add(new ValidationResultMessage { Code = message.Code, MessageText = message.MessageText });

            return this;
        }
    }

    public class ValidationResultMessage
    {
        public string Code { get; set; }
        public string MessageText { get; set; }
    }
}
