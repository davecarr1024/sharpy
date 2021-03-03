using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpy.Errors
{
    public class CompoundError : Error
    {
        public IEnumerable<Error> Errors { get; }

        private static Lexer.Location? MaxLocation(IEnumerable<Error> errors)
        {
            var locations = errors.Select(error => error.Location).Where(location => location != null);
            return locations.Any() ? locations.Max() : null;
        }

        private static IEnumerable<Error> MaxErrors(IEnumerable<Error> errors)
        {
            var max_loc = MaxLocation(errors);
            return errors.Where(error => error.Location.Equals(max_loc));
        }

        private static string FormatErrors(IEnumerable<Error> errors)
        {
            var max_errors = MaxErrors(errors);
            if (!max_errors.Any())
            {
                return "<unknown>";
            }
            else if (max_errors.Count() == 1)
            {
                return max_errors.First().ToString();
            }
            else
            {
                return string.Format("[{0}]", string.Join(", ", max_errors.Select(error => error.ToString())));
            }
        }

        public CompoundError(IEnumerable<Error> errors)
            : base(FormatErrors(MaxErrors(errors)), MaxLocation(errors))
            => Errors = MaxErrors(errors);

        public override bool Equals(object obj) => obj is CompoundError rhs && Errors.SequenceEqual(rhs.Errors) && base.Equals(rhs);

        public override int GetHashCode() => HashCode.Combine(Errors, base.GetHashCode());
    }
}