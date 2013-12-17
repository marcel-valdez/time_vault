require 'minitest/autorun'
require_relative '../lib/time_vault'

class TimeVaultTest < Minitest::Test
  def initialize(args)
    super(args)
  end

  def target(count = 1)
    TimeVault.new(count: count)
  end

  def test_get_transform_methods_str
    # arrange
    count = 20
    # act
    commands = target(count).transform_methods
    # assert
    puts "commands: #{commands}"
  end

  def test_get_transform_invocations
    # arrange
    count = 20
    # act
    invocations = target(count).transform_invocation
    # assert
    puts "invocations: #{invocations}"
  end

  def test_get_initial_str
    # arrange
    count = 20
    # act
    initial = target(count).initial_instruction
    # assert
    puts "initial_str: #{initial}"
  end
end