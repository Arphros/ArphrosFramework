using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArphrosFramework {
    internal interface IBackupable {
        public void Cache();
        public void Clear();
        public void Restore();
    }
}
