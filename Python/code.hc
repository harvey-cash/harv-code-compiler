// Increments val
def increment
    calc val = val + 1
end

check_exists check val
if check == 1
    call increment
end