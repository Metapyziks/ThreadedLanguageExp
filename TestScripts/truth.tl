bit a : false
bit b : false
bit c : false

byt ai
byt bi
byt ci

bit done : false

pln "a | b | c || Q"

whl !done
	bit Q : ( a & b ) | !( b & !c )
	bif a
		set ai : '1'
	els
		set ai : '0'
	end
	bif b
		set bi : '1'
	els
		set bi : '0'
	end
	bif c
		set ci : '1'
	els
		set ci : '0'
	end
	
	pln ai + " | " + bi + " | " + ci + " || " + Q
	
	set a : !a
	bif !a
		set b : !b
		bif !b
			set c : !c
			bif !c
				set done : true
			end
		end
	end
end

rky
