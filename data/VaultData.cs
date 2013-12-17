using System;

namespace TimeVault
{
  public class VaultData : IDisposable
  {
    private readonly string[] data;
    private bool disposed;

    public VaultData(string[] data)
    {
      this.data = data;
    }

    public VaultData()
    {
      this.data = new string[3];
    }

    public string Password
    {
      get
      {
        checkDisposed();
        return data[0];
      }

      set
      {
        checkDisposed();
      	data[0] = value;
      }
    }

    public double Deadline
    {
      get
      {
        checkDisposed();
        return Double.Parse(data[1].Trim());
      }

      set
      {
        checkDisposed();
      	data[1] = value.ToString();
      }
    }

    public double Waited
    {
      get
      {
        checkDisposed();
        return Double.Parse(data[2].Trim());
      }

      set
      {
        checkDisposed();
      	data[2] = value.ToString();
      }
    }

    private void checkDisposed()
    {
      if (disposed)
      {
        throw new ObjectDisposedException("data");
      }
    }

    ~VaultData()
    {
      this.Dispose();
    }

    public void Dispose()
    {
      this.data[0] = "_____invalidation____";
      this.data[1] = "_____invalidation____";
      this.data[2] = "_____invalidation____";
      this.disposed = true;
    }
  }
}
