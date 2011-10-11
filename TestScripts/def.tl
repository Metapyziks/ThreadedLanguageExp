int p_findPi_iters : 65536
dec r_findPi
fnc findPi
	dec value : 0

	int i : 0
	whl i < p_findPi_iters
		set value : value + 4.0 / ( 4.0 * i + 1.0 )
		set value : value - 4.0 / ( 4.0 * i + 3.0 )
		set i : i + 1
	end
	
	set r_findPi : value
end
