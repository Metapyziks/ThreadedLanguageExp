sub group
	prt "testA"
	prt "testB"
	grp
		int testInt : 5
		whl testInt > 0
			prt testInt
			set testInt : testInt - 1
		end
	end
	prt "testC"
	prt "testD"
end

prt "testing"
thr thread : group
prt "test1"
prt "test2"
prt "test3"
prt "test4"
prt "test5"
prt "test6"