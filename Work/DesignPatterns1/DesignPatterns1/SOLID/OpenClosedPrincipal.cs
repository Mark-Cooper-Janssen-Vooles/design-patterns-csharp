using System.Collections.Generic;

namespace DesignPatterns1.SOLID
{
    public enum Color
    {
        Red, Green, Blue
    }

    public enum Size
    {
        Small, Medium, Large, Huge
    }

    public class Product
    {
        public string Name;
        public Color Color;
        public Size Size;

        public Product(string name, Color color, Size size)
        {
            Name = name;
            Color = color;
            Size = size;
        }
    }

    public interface ISpecification<T> //works on anything
    {
        bool IsSatisfied(T t); //
    }

    public interface IFilter<T>
    {
        IEnumerable<T> Filter(IEnumerable<T> items, ISpecification<T> spec);
    }

    public class ColorSpecification : ISpecification<Product>
    {
        public Color color;

        public ColorSpecification(Color color)
        {
            this.color = color;
        }
        
        public bool IsSatisfied(Product t)
        {
            return t.Color == color;
        }
    }

    public class SizeSpecification : ISpecification<Product>
    {
        public Size Size;

        public SizeSpecification(Size size)
        {
            this.Size = size;
        }
        
        public bool IsSatisfied(Product t)
        {
            return t.Size == Size;
        }
    }

    public class BetterFilter : IFilter<Product>
    {
        public IEnumerable<Product> Filter(IEnumerable<Product> items, ISpecification<Product> spec)
        {
            foreach (var i in items)
                if (spec.IsSatisfied(i))
                    yield return i;
        }
    }

    public class AndSpecification<T> : ISpecification<T>
    {
        private ISpecification<T> _first, _second;

        public AndSpecification(ISpecification<T> first, ISpecification<T> second)
        {
            this._first = first;
            this._second = second;
        }
        public bool IsSatisfied(T t)
        {
            return _first.IsSatisfied(t) && _second.IsSatisfied(t);
        }
    }

    public class ProductFilter
    {
        public IEnumerable<Product> FilterBySize(IEnumerable<Product> product, Size size)
        {
            foreach (var p in product)
            {
                if (p.Size == size)
                    yield return p;
            }
        }
        
        public IEnumerable<Product> FilterByColor(IEnumerable<Product> product, Color color)
        {
            foreach (var p in product)
            {
                if (p.Color == color)
                    yield return p;
            }
        }
        
        public IEnumerable<Product> FilterBySizeAndColor(IEnumerable<Product> product, Color color, Size size)
        {
            foreach (var p in product)
            {
                if (p.Color == color && p.Size == size)
                    yield return p;
            }
        }
    }
}