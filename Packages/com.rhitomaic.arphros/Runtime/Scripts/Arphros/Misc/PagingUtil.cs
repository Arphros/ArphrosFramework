using System;
using System.Collections.Generic;
using System.Linq;

namespace ArphrosFramework {
    public class PagingUtil<T> {
        private readonly List<T> _sourceList;
        private readonly int _pageSize;

        public PagingUtil(List<T> list, int pageSize) {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");

            _sourceList = list;
            _pageSize = pageSize;
        }

        public int TotalPages => (int)Math.Ceiling((double)_sourceList.Count / _pageSize);

        public List<T> Get(int page) {
            if (page < 1 || page > TotalPages)
                throw new ArgumentOutOfRangeException(nameof(page), $"Page number must be between 1 and {TotalPages}.");

            int startIndex = (page - 1) * _pageSize;
            return _sourceList.Skip(startIndex).Take(_pageSize).ToList();
        }
    }
}