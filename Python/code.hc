check_exists check val
if check == 0
    set val = 1
end

calc val = val + 1
print val

if val < 10
    run "./code.hc"
end
