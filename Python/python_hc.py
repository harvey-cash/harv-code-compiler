# HarvCode: V0.1
class Methods:

    methods = {
        "set": lambda state_dict, methods_dict,  line, following_lines:
            Methods.set_cmd(state_dict, methods_dict, line, following_lines),

        "print": lambda state_dict, methods_dict,  line, following_lines:
            Methods.print_cmd(state_dict, methods_dict,  line, following_lines),

        "if": lambda state_dict, methods_dict,  line, following_lines:
            Methods.if_cmd(state_dict, methods_dict,  line, following_lines),

        "calc": lambda state_dict, methods_dict,  line, following_lines:
            Methods.calc_cmd(state_dict, methods_dict,  line, following_lines),

        "run": lambda state_dict, methods_dict,  line, following_lines:
            Methods.run_cmd(state_dict, methods_dict, line, following_lines),

        "exit": lambda state_dict, methods_dict, line, following_lines:
            Methods.exit_cmd(state_dict, methods_dict, line, following_lines),

        "check_exists": lambda state_dict, methods_dict, line, following_lines:
            Methods.check_exists_cmd(state_dict, methods_dict, line, following_lines),

        "def": lambda state_dict, methods_dict, line, following_lines:
            Methods.def_cmd(state_dict, methods_dict, line, following_lines),

        "call": lambda state_dict, methods_dict, line, following_lines:
        Methods.call_cmd(state_dict, methods_dict, line, following_lines),

        "//": lambda state_dict, methods_dict, line, following_lines:
        Methods.comment_cmd(state_dict, methods_dict, line, following_lines)
    }

    @staticmethod
    def set_cmd(state_dict, methods_dict, line, following_lines):
        state_dict[line[1]] = StringScript.parse_var(state_dict, line[3])
        return state_dict, following_lines

    @staticmethod
    def print_cmd(state_dict, methods_dict, line, following_lines):
        print(StringScript.parse_var(state_dict, line[1]))
        return state_dict, following_lines

    @staticmethod
    def comment_cmd(state_dict, methods_dict, line, following_lines):
        # Ignore anything on this line
        return state_dict, following_lines

    @staticmethod
    def if_cmd(state_dict, methods_dict, line, following_lines):
        if_dict, rest_of_script = StringScript.parse_if(state_dict, methods_dict, line, following_lines)

        p1 = if_dict.get("p1")
        op = if_dict.get("op")
        p2 = if_dict.get("p2")
        body = if_dict.get("body")

        if op(p1, p2):
            state_dict = StringScript.execute(body, methods_dict, state_dict)

        return state_dict, rest_of_script

    @staticmethod
    def calc_cmd(state_dict, methods_dict, line, following_lines):

        p1 = StringScript.parse_float(state_dict, line[3])
        op = StringScript.create_operator(line[4])
        p2 = StringScript.parse_float(state_dict, line[5])

        state_dict[line[1]] = op(p1, p2)

        return state_dict, following_lines

    @staticmethod
    def run_cmd(state_dict, methods_dict, line, following_lines):
        # words = StringScript.parse_script(open("./code.hc", "r"))

        path = line[1]
        if path[0] == "\"":
            path = path[1:len(path)-1]
        else:
            path = StringScript.parse_var(state_dict, path)
            path = path[1:len(path) - 1]

        load_script = StringScript.parse_script(open(path, "r"))
        state_dict = StringScript.execute(load_script, methods_dict, state_dict)

        return state_dict, following_lines

    @staticmethod
    def exit_cmd(state_dict, methods_dict, line, following_lines):
        # Skip the entire rest of the code
        state_dict["exit"] = 1
        return state_dict, []

    @staticmethod
    def check_exists_cmd(state_dict, methods_dict, line, following_lines):
        state_dict[line[1]] = int(state_dict.get(line[2]) is not None)
        return state_dict, following_lines

    @staticmethod
    def def_cmd(state_dict, methods_dict, line, following_lines):

        code_body, rest_of_script = StringScript.parse_def(state_dict, methods_dict, line, following_lines)
        state_dict[line[1]] = code_body

        return state_dict, rest_of_script

    @staticmethod
    def call_cmd(state_dict, methods_dict, line, following_lines):
        # words = StringScript.parse_script(open("./code.hc", "r"))

        script = state_dict.get(line[1])
        state_dict = StringScript.execute(script, methods_dict, state_dict)

        return state_dict, following_lines


class Runtime:

    def __init__(self):
        print()
        print("HarvCode: V0.1")
        print("~~~~~~~~~~~~~~")
        self.state = {
            "exit": 0
        }
        self.methods = Methods.methods

    def on_start(self, script):
        self.state = StringScript.execute(script, self.methods, self.state)
        return self.state

    def run_terminal(self):
        while self.state.get("exit") == 0:
            script_string = input("> ")
            if script_string == "stop" or script_string == "exit":
                break
            else:
                script = StringScript.parse_script([script_string])
                self.state = StringScript.execute(script, self.methods, self.state)
                print(repr(self.state))

        return self.state


class StringScript:

    @staticmethod
    def parse_script(string):
        lines = []
        for line in string:
            if line == "" or line == "\n":
                pass

            # Eliminate odd characters
            l_words = StringScript.parse_line(line)

            if len(l_words) > 0:
                lines.append(l_words)

        return lines

    @staticmethod
    def parse_line(line):

        line_words = []
        within_string = False
        w_buffer = ""

        for i, c in enumerate(line):
            if c == "\"":
                # flip string status
                within_string = not within_string

                # If word finished, ship it off.
                w_buffer += c
                if not within_string:
                    line_words.append(w_buffer)
                    w_buffer = ""

            elif c == " ":
                if within_string:
                    w_buffer += c
                elif len(w_buffer) > 0:
                    line_words.append(w_buffer)
                    w_buffer = ""

            elif c != "\n":
                w_buffer += c

        if len(w_buffer) > 0:
            line_words.append(w_buffer)

        return line_words

    @staticmethod
    def execute(script, methods_dict, state_dict):
        # End of script
        if len(script) < 1:
            return state_dict

        # Finish flag set
        if not state_dict.get("exit") == 0:
            return state_dict

        # Lines left to run
        line = script[0]
        following_lines = script[1:len(script)]

        function = methods_dict.get(line[0])
        if function is not None:
            state_dict, rest_of_script = methods_dict.get(line[0])(state_dict, methods_dict,  line, following_lines)
        else:
            print("\"" + line[0] + "\" is undefined.")
            rest_of_script = following_lines

        return StringScript.execute(rest_of_script, methods_dict, state_dict)

    @staticmethod
    def parse_var(state_dict, name):
        if name[0] == "\"" and name[len(name)-1] == "\"":
            return name[1:len(name)-1]
        elif str.isnumeric(name):
            return float(name)
        else:
            return state_dict.get(name)

    @staticmethod
    def parse_float(state_dict, name):
        if StringScript.is_float(name):
            return float(name)
        else:
            return float(state_dict.get(name))

    @staticmethod
    def is_float(string):
        try:
            float(string)
            return True
        except ValueError:
            return False

    @staticmethod
    def parse_if(state_dict, methods_dict, line, following_lines):

        p1 = StringScript.parse_float(state_dict, line[1])
        p2 = StringScript.parse_float(state_dict, line[3])
        op = StringScript.create_operator(line[2])

        if_dict = {
            "p1": p1,
            "op": op,
            "p2": p2
        }

        # Keep track of the depth of if statements!
        if_end = 0
        depth_counter = 0
        for (j, line) in enumerate(following_lines):
            if line[0] == "def" or line[0] == "if":
                depth_counter += 1
            elif depth_counter > 0 and line[0] == "end":
                depth_counter -= 1
            elif line[0] == "end":
                if_end = j
                break

        body = following_lines[0:if_end]
        rest_of_script = following_lines[if_end+1:len(following_lines)]

        if_dict["body"] = body
        return if_dict, rest_of_script

    @staticmethod
    def parse_def(state_dict, methods_dict, line, following_lines):

        # Keep track of the depth of if statements!
        def_end = 0
        depth_counter = 0
        for (j, line) in enumerate(following_lines):
            if line[0] == "def" or line[0] == "if":
                depth_counter += 1
            elif depth_counter > 0 and line[0] == "end":
                depth_counter -= 1
            elif line[0] == "end":
                def_end = j
                break

        body = following_lines[0:def_end]
        rest_of_script = following_lines[def_end + 1:len(following_lines)]

        return body, rest_of_script

    @staticmethod
    def create_operator(opstr):
        if opstr == "==":
            def op(a, b): return a == b
        elif opstr == "!=":
            def op(a, b): return a != b
        elif opstr == ">":
            def op(a, b): return a > b
        elif opstr == ">=":
            def op(a, b): return a >= b
        elif opstr == "<":
            def op(a, b): return a < b
        elif opstr == "<=":
            def op(a, b): return a <= b
        elif opstr == "+":
            def op(a, b): return a + b
        elif opstr == "-":
            def op(a, b): return a - b
        elif opstr == "/":
            def op(a, b): return a / b
        elif opstr == "*":
            def op(a, b): return a * b
        else:
            print("Undefined operator: \"" + opstr + "\"")
            def op(a, b): return False

        return op


run = Runtime()

# words = StringScript.parse_script(open("./code.hc", "r"))
run.run_terminal()
