namespace _7Colors.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            //ProductTypes = new ProductTypesRepository(_db);
            //SpecialTags = new SpecialTagRepository(_db);
        }
        //public IProductTypesRepository ProductTypes { get; private set; }

        //public ISpecialTagRepository SpecialTags { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
