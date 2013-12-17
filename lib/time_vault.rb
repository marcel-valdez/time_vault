class TimeVault
  attr_reader :count, :initial_str
  def initialize(options = {})
    if options[:count].nil?
      @count = 1000
    else
      @count = options[:count]
    end
    @initial_str = random_str(32)
    init_transforms()
  end

  def initial_instruction
    "string str = \"#{initial_str}\";"
  end

  def transform_methods
    (0..@count).to_a.collect {|i|
      "private static string Transform#{i}(string str) {" +
          alternatives[rand(alternatives.length)].call +
      '}'
    }.join("\n")
  end

  def transform_invocation
    (0..@count).to_a.inject('str') {|accum, i|
      "Transform#{i}(#{accum})"
    }
  end

  private

  attr_accessor :alternatives, :transforms

  def init_transforms
    @transforms = []
    @alternatives = [
        lambda {
          replace_a_start = rand(30)
          replace_len = rand(31 - replace_a_start) + 1
          replace_b_start = rand(replace_a_start)

          "string subA = str.Substring(#{replace_a_start}, #{replace_len});" +
          "string subB = str.Substring(#{replace_b_start}, #{replace_len});" +
          'return str.Replace(subA, subB);'
        },

        lambda { "return str.Replace('#{random_char()}', '#{random_char}');" },

        lambda {
                'var sb = new StringBuilder(str);' +
                "int a = #{rand(31)};" +
                 "int b = #{rand(31)};" +
                'char x = str[a];' +
                'sb[a] = sb[b];' +
                'sb[b] = x;' +
                'return sb.ToString();'},

        lambda {
          replaced_start = rand(30)
          replaced_len = rand(31 - replaced_start)

          "string subA = str.Substring(#{replaced_start}, #{replaced_len + 1});" +
          "return str.Replace(subA, \"#{random_str(replaced_len + 1)}\");"
        }
    ]
  end

  def random_from(numbers = [])
    numbers[rand(numbers.length)]
  end

  def random_char
    random_str(1)
  end

  def random_str(len = 1)
    chars = 'abcdefghjkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ0123456789!@#$%^&*()-='
    (0...len).map{ chars[rand(chars.length)] }.join
  end

end
