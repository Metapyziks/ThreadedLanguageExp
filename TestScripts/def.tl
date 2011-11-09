int piIters : 65536
dec pi 		: 0.0
sub findPiPositive
	int i : 0
	whl i < piIters
		set pi : pi + 4.0 / ( 4.0 * i + 1.0 )
		set i : i + 1
	end
	
	syn main
end
sub findPiNegative
	int i : 0
	whl i < piIters
		set pi : pi - 4.0 / ( 4.0 * i + 3.0 )
		set i : i + 1
	end
	
	syn main
end

thr piPosThread : findPiPositive
thr piNegThread : findPiNegative
prt "Calculating Pi..."
syn piPosThread
syn piNegThread
int rounded : pi * 10000.0 + 0.5
prt "Done! Pi = " + ( 0.0001 * rounded )
