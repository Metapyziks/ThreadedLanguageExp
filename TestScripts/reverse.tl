str input : "Hello world!"
str buffer
str output

byt byte

whl input
	inp byte : input
	
	bif input
		out buffer : byte
	els
		out output : byte
		set input : buffer
		set buffer : ""
	end
end

prt output
