# TIME VAULT

What is it for?
---------------
The best way to explain it is via a story:

Johnny is an avid online gamer, but he also needs to take care of his RL (real life), but can't seem to stop playing.

So Johnny finds out that he is somewhat... addicted... no that is too strong a word, he just likes playing games ALOT.

This is where Time Vault helps, in order for Johnny to lock himself out of his game, he has to do 2 things:

1. Change his e-mail account associated to the game to an e-mail account he can be without for the period he will lock himself.

2. Change the password of that e-mail account to something he can't possibly remember, something like: Mpkhj81N0aj%@!Z, then `Ctrl+C` that password.

3. Then Johnny would download the out/TimeVault.exe in this repository.

4. Then Johnny would run cmd.exe and CD to the directory where he downloaded TimeVault.exe.

5. Assume that today is Dec. 16 of 2013, and Johnny wants to lock himself until Dec. 24th. Then he would need to run the following: `TimeVault.exe -n "Mpkhj81N0aj%@!Z" "24/12/2013 00:00"`

6. Now Johnny must close the cmd.exe window, copy some other text (in order to delete the password from the clipboard) and re-open cmd.exe and then run this command: `TimeVault.exe -r`

7. Then, assuming that he does not close the cmd.exe window until Dec 24th, the console will print the password to the console.

Now, in order for the program to finish at exactly 00:00 on Dec. 24th, he needs to KEEP THE PROGRAM RUNNING, if he closes the program it will stop decreasing the counter to Dec. 24th.

If Johnny closes the cmd.exe window running TimeVault, he can simply reopen the window and run the same command again, he will only lose 1 minute's worth of wait time.

Basically, Johnny's computer MUST NOT hibernate or shutdown until Dec. 24th. If he were to be expecting downtime (lets say his computer is ON only 8 hrs a day, then he can do the math of setting the 'release date' to 1/3 of the days to Dec. 24th, i.e. 2.5 days, approx for the example)

Another feature of TimeVault is the capacity to add days to the wait time, so for example Johnny can add 24 hours to the wait time by doing in cmd.exe: `TimeVault.exe -i 24 h`

This adds 7 days: `TimeVault.exe -i 7 d`

This adds 30 minutes: `TimeVault.exe -i 30 m`
 
You must of course, stop the program (`Ctrl+C`) and then run this command, then it will continue waiting (with the added wait time).



How does Time Vault work?
-------------------------
Time Vault is made for the addicted. It is a program that will store a string in an encrypted file, and will wait for X amount of minutes, hours or days to print out to the console that encrypted string.

The whole point of Time Vault is that you store this string and can't retrieve it. The code that generates the key used to encrypt the message is obfuscated, uses encrypted strings as seed values that then are transformed by 30 compile-time generated functions, on each build these 30 functions are different, and can be configured to be 100, 1000 ... whatever your stack allows.

## For the powerusers, coder, hackers, crackers: 

How to create a unique TimeVault.exe instance?
----------------------------------------------

### Requirements
1. Ruby: http://rubyinstaller.org/
2. .Net 4.0: http://www.microsoft.com/en-us/download/details.aspx?id=17851
3. Git: http://git-scm.com/download/win

Assuming you installed all these programs, then you must do the following:

1. Open Git Bash
2. `git clone git@github.com:marcel-valdez/time_vault.git`
3. `cd time-vault`
4. `bundle install`
5. `bundle exec rake build_program`
6. `cd out`
7. `TimeVault.exe "test123" "11/11/2011 11:11"` in order to be sure that it was generated correctly.
 
