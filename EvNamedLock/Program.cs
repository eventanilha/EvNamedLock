using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EvNamedLock
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 100; i++)
            {
                RunWithNameLock();
                RunWithLock();
            }           
        }

        static void RunWithLock()
        {
            IEnumerable<Contract> contracts = new List<Contract> { new Contract { Id = 1 },
                                                                   new Contract { Id = 2 },
                                                                   new Contract { Id = 3 } };

            Stopwatch sw = new Stopwatch();

            try
            {
                sw = Stopwatch.StartNew();

                ContractService.Load(contracts);

                Parallel.Invoke(new Action[]
                {
                    () => ContractService.AuthorizeLock(1, true),
                    () => ContractService.AuthorizeLock(2, false),
                    () => ContractService.AuthorizeLock(1, true),
                    () => ContractService.AuthorizeLock(3, false),
                    () => ContractService.AuthorizeLock(2, true),
                    () => ContractService.AuthorizeLock(1, false),
                    () => ContractService.AuthorizeLock(3, true),
                    () => ContractService.AuthorizeLock(1, true),
                    () => ContractService.AuthorizeLock(1, true)

                });
            }
            catch (Exception e)
            {
                if (e is AggregateException)
                {
                    var errors = (AggregateException)e;
                    foreach (var item in errors.InnerExceptions)
                    {
                        //Console.WriteLine(e.InnerException.Message);
                    }
                }
                //else
                   //Console.WriteLine(e.Message);
            }
            finally
            {
                //Console.WriteLine();
                Console.WriteLine("Time elapsed with Lock: {0} ms", sw.ElapsedMilliseconds);
                //Console.WriteLine("Contract Id {0} - Approval status: {1}", 1, ContractService.GetById(1).Approval);
                //Console.WriteLine("Contract Id {0} - Approval status: {1}", 2, ContractService.GetById(2).Approval);
                //Console.WriteLine("Contract Id {0} - Approval status: {1}", 3, ContractService.GetById(3).Approval);
                //Console.WriteLine();
            }
        }

        static void RunWithNameLock()
        {
            IEnumerable<Contract> contracts = new List<Contract> { new Contract { Id = 1 },
                                                                   new Contract { Id = 2 },
                                                                   new Contract { Id = 3 } };

            Stopwatch sw = new Stopwatch();

            try
            {
                sw = Stopwatch.StartNew();

                ContractService.Load(contracts);

                Parallel.Invoke(new Action[]
                {
                    () => ContractService.AuthorizeNamedLock(1, true),
                    () => ContractService.AuthorizeNamedLock(2, false),
                    () => ContractService.AuthorizeNamedLock(1, true),
                    () => ContractService.AuthorizeNamedLock(3, false),
                    () => ContractService.AuthorizeNamedLock(2, true),
                    () => ContractService.AuthorizeNamedLock(1, false),
                    () => ContractService.AuthorizeNamedLock(3, true),
                    () => ContractService.AuthorizeNamedLock(1, true),
                    () => ContractService.AuthorizeNamedLock(1, true)

                });
            }
            catch (Exception e)
            {
                if (e is AggregateException)
                {
                    var errors = (AggregateException)e;
                    foreach (var item in errors.InnerExceptions)
                    {
                        //Console.WriteLine(e.InnerException.Message);
                    }
                }
                //else
                   // Console.WriteLine(e.Message);
            }
            finally
            {
                //Console.WriteLine();
                Console.WriteLine("Time elapsed with Named Lock: {0} ms", sw.ElapsedMilliseconds);
                //Console.WriteLine("Contract Id {0} - Approval status: {1}", 1, ContractService.GetById(1).Approval);
                //Console.WriteLine("Contract Id {0} - Approval status: {1}", 2, ContractService.GetById(2).Approval);
                //Console.WriteLine("Contract Id {0} - Approval status: {1}", 3, ContractService.GetById(3).Approval);
                //Console.WriteLine();
            }
        }
    }

    internal class Contract
    {
        public int Id { get; set; }
        public bool? Approval { get; set; }
    }

    internal static class ContractService
    {
        static readonly NamedLock namedLock = new NamedLock();

        static object thisLock = new object();

        static IEnumerable<Contract> context;
        public static void Load(IEnumerable<Contract> contracts)
        {
            if (contracts != null)
                context = contracts;
        }

        public static Contract GetById(int id)
        {
            return context.Where(i => i.Id.Equals(id)).FirstOrDefault();
        }

        public static void AuthorizeNamedLock(int id, bool approval)
        {
            namedLock.RunWithNamedLock(id.ToString(), () =>
            {
                var contract = GetById(id);

                Validade(contract);

                if (contract != null)
                    contract.Approval = approval;

                //Console.WriteLine("Contract approved");

            });
        }

        public static void AuthorizeLock(int id, bool approval)
        {
            lock (thisLock)
            {
                var contract = GetById(id);

                Validade(contract);

                if (contract != null)
                    contract.Approval = approval;

                //Console.WriteLine("Contract approved");

            }
        }

        private static void Validade(Contract contract)
        {
            if (contract == null)
                throw new ArgumentNullException("Contract not found");

            if (contract.Id == 0)
                throw new Exception("Contract invalid");

            if (contract.Approval != null)
                throw new Exception("Contract already approved");
        }
    }
}
