using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HelloOptions
{
    public class ApplicationOptions
    {
        public const string SectionName = "Application";

        [StringLength(2, ErrorMessage ="The application name length must less than or equal to 2.")]
        public string Name { get; set; }
        public TimeSpan CacheLifetime { get; set; }

        public IEnumerable<string> KnownAnimals { get; set; }
    }
}
