class ProgramContext
  attr_accessor :time_vault
  def initialize(time_vault)
    @time_vault = time_vault
  end

  def generate_key_getter
"using System.Text;

namespace TimeVault
{
  public class KeyGetter
  {
    public static string GetKey()
    {
      #{@time_vault.initial_instruction}
      return #{@time_vault.transform_invocation};
    }

    #{time_vault.transform_methods}
  }
}
"
  end

  def generate_dummy_key_getter
"namespace TimeVault
{
  public class KeyGetter
  {
    public static string GetKey()
    {
      return \"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\";

    }
  }
}
"
  end
end