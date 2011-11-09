str inputA : "01010110"
str inputB : "01110101"

pln "  " + inputA
pln "+ " + inputB
pln "  --------"

str reverse_in
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

set reverse_in : inputA
cal reverse
set inputA : reverse_in
set reverse_in : inputB
cal reverse
set inputB : reverse_in

str output

bit carry : false

whl inputA & inputB
	byt aByte
	byt bByte
	
	inp aByte : inputA
	inp bByte : inputB
	
	bit a : aByte == '1'
	bit b : bByte == '1'
	
	bif carry
		bif a ^ b
			out output : '0'
			set carry : true
		eif a & b
			out output : '1'
			set carry : true
		els
			out output : '1'
			set carry : false
		end
	els
		bif a ^ b
			out output : '1'
		eif a & b
			out output : '0'
			set carry : true
		els
			out output : '0'
		end
	end
end

bif carry
	out output : '1'
end

set reverse_in : output
cal reverse
set output : reverse_in

pln "= " + output

rky
