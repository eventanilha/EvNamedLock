# EvNamedLock
A Named Lock in C#

Usage:

    static readonly NamedLock namedLock = new NamedLock();
    
    public void Foo(string key)
    {
       namedLock.RunWithNamedLock(key, () =>
       {
          //Action
       });
    }
