# EvNamedLock
A Named Lock in C#

This is useful for situations where there is a potentially infinite number of resources (e.g., database queries) that should not be used concurrently.

Usage:

    static readonly NamedLock namedLock = new NamedLock();
    
    public void Foo(string key)
    {
       namedLock.RunWithNamedLock(key, () =>
       {
          //Action
       });
    }
