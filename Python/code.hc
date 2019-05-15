set depth_counter = 0
set depth_limit = 7

// Increments val
def increment
    calc val = val + 1
    if val < 10
        calc depth_counter = depth_counter + 1

        if depth_counter <= depth_limit
            print "Calling increment recursively."
            call increment
        end
        if depth_counter > depth_limit
            print "Recurse depth exceeds limit: "
            print depth_counter
        end
    end
end

def run_code
    run "./code.hc"
end

check_exists check val
if check == 1
    call increment
end
if check != 1
    set val = 1
end