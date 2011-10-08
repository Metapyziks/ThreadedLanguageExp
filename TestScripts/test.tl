int testVar : 5

str testStream

fnc testFunc( param )
	set param : param + 5
	rtn param
end

thr testThread
	whl testVar < 25
		set testVar : testFunc( testVar )
	end
	
	out testStream : [02]
	
	syn main
end

sta testThread

grp
	bif testVar < 15
		out testStream : [01]
	end
end

syn testThread