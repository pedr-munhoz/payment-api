using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace payment_api.Models
{
    /// <summary>
    /// Class responsible for checking if all the required fields are set on any given entity.
    /// </summary>
    /// <typeparam name="T">The type of the entity to be validated.</typeparam>
    public class EntityCreationResult<T>
    {
        public EntityCreationResult(T value)
        {
            var result = new List<ValidationResult>();
            var context = new ValidationContext(value, null, null);

            Success = Validator.TryValidateObject(value, context, result, true);
            Context = context;

            if (Success)
                Value = value;
        }

        public T Value { get; }
        public bool Success { get; }
        public ValidationContext Context { get; }
    }
}