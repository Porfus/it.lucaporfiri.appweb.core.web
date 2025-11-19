using System.ComponentModel.DataAnnotations;

namespace it.lucaporfiri.appweb.core.web.Utilities
{
    public static class ModelValidatorExtensions
    {
        public static void ValidateObject(object model)
        {
            if (model is null) throw new ArgumentNullException(nameof(model));

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);

            if (!isValid)
            {
                var messages = results.Select(r => r.ErrorMessage).Where(m => !string.IsNullOrWhiteSpace(m));
                throw new ValidationException(string.Join("; ", messages));
            }
        }
    }
}