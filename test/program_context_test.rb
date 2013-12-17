require 'minitest/autorun'
require_relative '../lib/program_context'
require_relative '../lib/time_vault'

class TimeVaultTest < Minitest::Test
  def initialize(args)
    super(args)
  end

  def test_key_getter
    # arrange
    target = ProgramContext.new(TimeVault.new({count: 20}))
    # act
    actual = target.generate_key_getter
    # assert
    puts actual
  end
end