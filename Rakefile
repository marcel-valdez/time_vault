require 'rubygems/package_task'
require 'rake/clean'
require 'rake/testtask'
require_relative 'lib/time_vault'
require_relative 'lib/program_context'

desc 'Build the obfuscated .Net program'
task :build_program do
  puts 'Building .Net program'
  generator = ProgramContext.new(TimeVault.new({count: 20}))
  key_getter = generator.generate_key_getter

  File.open('data/KeyGetter.cs', 'w') {|file|
    file.write(key_getter)
  }

  if system 'data/build.bat'
    puts 'Deleting evidence'
    File.open('data/KeyGetter.cs', 'w') { |file|
      file.write(generator.generate_dummy_key_getter) # delete evidence by rewriting
    }

    system 'data/build.bat -no-confuse'
  end
end

desc 'Test task'
Rake::TestTask.new do |t|
  files = FileList['test/*_test.rb']
  t.loader = :rake
  t.test_files = files
  t.warning = true
end