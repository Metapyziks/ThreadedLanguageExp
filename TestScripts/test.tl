int testVar : 5
str testStream
fnc testFunc : param
	rtn param + 5
end
bit finish : false
thr testThread
	whl testVar < 25
		cal testFunc
	end
	out testStream : [02]
	set finish : true
	syn main
end
sta testThread
whl finish = false
	grp
		bif testVar < 15
			out testStream : [01]
		end
	end
end
syn testThread
