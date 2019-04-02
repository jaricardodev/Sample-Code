using System;
using System.Linq;

namespace LISTING_1_10_Exceptions_in_PLINQ
{
    class Program
    {
        public static bool CheckCity(string name)
        {
            if (name == "")
                throw new ArgumentException(name);
            return name == "Seattle";
        }

        class Person
        {
            public string Name { get; set; }
            public string City { get; set; }
        }

        static void Main(string[] args)
        {
            Person[] people = new Person[] {
                new Person { Name = "Alan", City = "Hull" },
                new Person { Name = "Beryl", City = "Seattle" },
                new Person { Name = "Charles", City = "London" },
                new Person { Name = "David", City = "Seattle" },
                new Person { Name = "Eddy", City = "" },
                new Person { Name = "Fred", City = "" },
                new Person { Name = "Gordon", City = "Hull" },
                new Person { Name = "Henry", City = "Seattle" },
                new Person { Name = "Isaac", City = "Seattle" },
                new Person { Name = "James", City = "London" }};

            var result1 = from person in people.AsParallel()
                         where person.City == "Seattle"
                         select person;
            /*
            var result2 = people.AsParallel()
                                .Where(x=>x.City == "Seattle")
                                .Select(x=>x.City);
            */
            //Same as before but using chainig method

            /*
             The AsParallel method examines the query to determine if using a parallel version would speed it up.
             If the AsParallel method can’t decide whether parallelization would improve performance the query is not 
                executed in parallel.
             */

            //ForAll method can be used to iterate through all of the elements in a query. 
            //It will not reflect the ordering of the input data
            result1.ForAll(person => Console.WriteLine(person.Name));
            Console.WriteLine("AsParallel executed");
            Console.ReadKey();

            //Paralelism here is enforced regardless of optimization
            var result3 = from person in people.AsParallel().
             WithDegreeOfParallelism(4).
             WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                         where person.City == "Seattle"
                         select person;            

            foreach (var person in result3)
                Console.WriteLine(person.Name);
            Console.WriteLine("AsParallel using WithDegreeOfParallelism and WithExecutionMode executed");
            Console.ReadKey();
                       
            // A parallel query, may process data in a different order from the input data. To prevent this use AsOrdered()
            // It can slow down the query
            var result4 = from person in
             people.AsParallel().AsOrdered()
                         where person.City == "Seattle"
                         select person;

            foreach (var person in result4)
                Console.WriteLine(person.Name);
            Console.WriteLine("AsParallel using AsOrdered executed");
            Console.ReadKey();
            
            //Here ordering is preserved by the use of AsSequential before the Take
            var result5 = (from person in people.AsParallel()
                          where person.City == "Seattle"
                          orderby (person.Name)
                          select new
                          {
                              person.Name
                          }).AsSequential().Take(4);


            
            Console.WriteLine("AsParallel using AsSequential executed");
            Console.ReadKey();



            foreach (var person in result5)
                Console.WriteLine(person.Name);
            Console.WriteLine("AsParallel using WithDegreeOfParallelism and WithExecutionMode executed");
            Console.ReadKey();

            //If any queries generate exceptions an AgregateException will be thrown when the query is complete.
            try
            {
                var result = from person in
                    people.AsParallel()
                             where CheckCity(person.City)
                             select person;
                result.ForAll(person => Console.WriteLine(person.Name));
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e.InnerExceptions.Count + " exceptions.");
            }

            Console.WriteLine("Finished processing. Press a key to end.");
            Console.ReadKey();
        }
    }
}
