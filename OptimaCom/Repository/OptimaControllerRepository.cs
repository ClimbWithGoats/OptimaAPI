using OptimaCom.Repository;
using System;

namespace OptimaCom
{
    public class OptimaControllerRepository : BaseRepository, IDisposable
    {
        public OptimaControllerRepository(string conn) : base(conn)
        {
        }

        public void Dispose()
        {
        }

        public override object GetData(object name)
        {
            throw new System.NotImplementedException();
        }

        public override int GetItemById(object obj)
        {
            throw new System.NotImplementedException();
        }

        internal void AddAttribute(string akronim = "")
        {
         //   OptimaController.OptimaWrapper.AddOrUpdateContractor();
            Console.WriteLine("Dodawanie atrybutu dla: " + akronim);
        }
    }
}