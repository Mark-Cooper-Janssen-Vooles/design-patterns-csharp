namespace DesignPatterns1.SOLID
{
    public class Document
    {
        
    }
    
    
    public class MultifunctionPrinter : IMultiFunctionDevice
    {
        public void Scan(Document d)
        {
            //
        }

        public void Print(Document d)
        {
            //
        }

        public void Fax(Document d)
        {
            //
        }
    }
    
    public class OldPrinter : IPrinter
    {
        public void Print(Document d)
        {
            //
        }
    }

    public interface IPrinter
    {
        void Print(Document d);
    }

    public interface IScanner
    {
        void Scan(Document d);
    }

    public interface IFax
    {
        void Fax(Document d);
    }
    
    //can do this:
    public class Photocopier : IPrinter, IScanner
    {
        public void Print(Document d)
        {
            //
        }

        public void Scan(Document d)
        {
            //
        }
    }
    
    //or this:
    public interface IMultiFunctionDevice : IScanner, IPrinter, IFax
    {
        
    }
}