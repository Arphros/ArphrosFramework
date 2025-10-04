namespace ArphrosFramework {
    internal interface IBackupable {
        public void Cache();
        public void Clear();
        public void Restore();
    }
}
