# SandSharp Language
Intended for writing and running within games, Sand Sharp is somewhere between python
and javascript in syntax, yet compiles to C# (python is on the way, though).

`# SandSharp Example Script

# SandSharp is space (' ') and tab agnostic
# Commands are separated by newlines
# Method lines don't NEED indentations, but its
# obviously good style

# Single command methods can go on one line
def hello() { print("Hello World!") }

def run(func) {
	# but generally you should use multiple lines
	func()
}

# functions can be passed as parameters
run(hello)

# Anything after a '#' is ignored until the next newline
# print("This isn't printed!")

# variables are dynamically typed
counter = 1

while(counter <= 3) {
	print("Counter: ", counter)
	# (can't concatenate strings yet, but print() can
	# take multiple arguments)

	counter = counter + 1
}

# Anything defined locally stays local
def testLocalScope() {
	p1 = 4
	p2 = 5

	# methods can be defined within methods
	# return doesn't exit control of the function
	# and as such the notation uses '=' assignment
	def average(a, b) {
		return = (a + b) / 2
	}

	# method calls can be passed as parameters
	print("Average of ", p1, " and ", p2, " = ", average(p1,p2))

}

testLocalScope()

# p1 and p2 were defined locally, so...
print(p1) # throws an error...

# Which stops script execution!
print("This won't get printed!")`
