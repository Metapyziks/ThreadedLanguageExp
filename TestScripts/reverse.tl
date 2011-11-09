str reverse_in : "Hello world!"
sub reverse
	str output
	str buffer
	
	byt byte

	whl reverse_in
		inp byte : reverse_in
		
		bif reverse_in
			out buffer : byte
		els
			out output : byte
			set reverse_in : buffer
			set buffer : ""
		end
	end
	
	set reverse_in : output
end
