import subprocess, sys

if sys.platform.startswith("win"):
    os_split = ";";
else:
    os_split = ":";

libaries = list();
libaries.append("-classpath \"." + os_split);
libaries.append("../../../../lib/servlet-api.jar" + os_split);
libaries.append("../../../../lib/smtp.jar" + os_split);
libaries.append("../../../../lib/jsp-api.jar" + os_split);
libaries.append("\" ");

String = "javac ";
for i in range(0, len(libaries)):
    String += libaries[i];
String += "*.java";

proc = subprocess.Popen(String, shell=True)
proc.wait()
