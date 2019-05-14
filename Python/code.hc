set val_1 = 3
set val_2 = 3

calc val_3 = val_1 * val_2
print val_3

if val_1 >= val_2
    print "val_1 is greater than or equal to val_2!"
    if val_1 == val_2
        print "val_1 and val_2 are the same!"
        if val_2 >= 4
            print "val_2 is greater than or equal to 4!"
        end
    end
end
if val_1 < val_2
    print "val_1 is less than val_2!"
end