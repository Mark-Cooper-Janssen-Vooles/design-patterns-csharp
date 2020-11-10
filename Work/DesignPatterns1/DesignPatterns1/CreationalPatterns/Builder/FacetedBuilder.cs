using System;

namespace DesignPatterns1.CreationalPatterns.Builder
{
    public class Person3
    {
        //address
        public string StreetAddress, Postcode, City; //might want a builder just for the address
        //employement info
        public string CompanyName, Position;
        public int AnnualIncome;
        
        public override string ToString()
        {
            return $"{nameof(StreetAddress)}: {StreetAddress}, {nameof(Postcode)}: {Postcode}, {nameof(City)}: {City}, " +
                   $"{nameof(CompanyName)}: {CompanyName}, {nameof(Position)}: {Position}, {nameof(AnnualIncome)}: {AnnualIncome}";
        }
    }
    
    //a facet
    public class PersonBuilder3 //facade
    {
        //reference type. If you have a value type you could be in trouble, so always work with objects 
        protected Person3 person = new Person3();
        
        //expose personJobBuilder3
        public PersonJobBuilder3 Works => new PersonJobBuilder3(person);
        public PersonAddressBuilder3 Lives => new PersonAddressBuilder3(person);

        //this function allows you to return a person when implicitly called with "Person3 person = new PersonBuilder3();"
        //if you use "var person = new PersonBuilder3();" it will just return  a PersonBuilder3.
        public static implicit operator Person3(PersonBuilder3 pb)
        {
            return pb.person;
        }
    }

    public class PersonJobBuilder3 : PersonBuilder3
    {
        //building up job information on a person3 object
        public PersonJobBuilder3(Person3 person)
        {
            this.person = base.person;
        }

        public PersonJobBuilder3 At(string companyName)
        {
            person.CompanyName = companyName;
            return this;
        }

        public PersonJobBuilder3 AsA(string position)
        {
            person.Position = position;
            return this;
        }

        public PersonJobBuilder3 Earning(int amount)
        {
            person.AnnualIncome = amount;
            return this;
        }
    }

    public class PersonAddressBuilder3 : PersonBuilder3
    {
        public PersonAddressBuilder3(Person3 person)
        {
            this.person = base.person;
        }

        public PersonAddressBuilder3 StreetAddress(string address)
        {
            person.StreetAddress = address;
            return this;
        }

        public PersonAddressBuilder3 Postcode(string postcode)
        {
            person.Postcode = postcode;
            return this;
        }

        public PersonAddressBuilder3 City(string city)
        {
            person.City = city;
            return this;
        }
    }
    
    
    // class Program
    // {
    //     static void Main(string[] args)
    //     {
    //         var pb = new PersonBuilder3();
    //         Person3 person = pb
    //             .Works
    //                 .At("Xero")
    //                 .AsA("Developer")
    //                 .Earning(99999)
    //             .Lives
    //                 .StreetAddress("123 Fake Street")
    //                 .Postcode("1234")
    //                 .City("Melbourne");
    //         Console.WriteLine(person);
    //     }
    // }
}