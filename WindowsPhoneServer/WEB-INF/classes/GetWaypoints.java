import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import java.io.*;
import java.util.*;

public class GetWaypoints extends HttpServlet
{
    private static String path = "../webapps/WindowsPhoneServer/data/";

    public void doPost(HttpServletRequest request, HttpServletResponse response)
    {
        // Read all the variables from the url arguments
        String userID    = request.getParameter("userID");
        String routeID   = request.getParameter("routeID");
        // Sanity check, that noone of them are null
        if ((userID == null) || (routeID == null))
        {
            // TODO: Possibly return a error value, instead of just returning nothing
            System.err.println("NULL ARGUMENTS");
            return;
        }
        // 1. Lookup the folder with the name UserIDString
        String folder_path = path + userID + "/";
        File folder = new File(folder_path);
        // 1a. If this folder does not exist, return an error value (“NO SUCH USERID”)
        if (!folder.exists())
        {
            System.err.println("NO SUCH USERID");
        }
        // 2. Lookup the file, within this folder, with the name RouteIDString
        String file_path = folder_path + routeID + ".log";
        File file = new File(file_path);
        // 2a. If this file does not exist, return an error value (“NO SUCH ROUTEID”)
        if (!file.exists())
        {
            System.err.println("NO SUCH ROUTEID");
        }
        // 3. Read all the line of this file
        // A list to put the resulting string into
        List<String> lines = new ArrayList<String>();
        // Loop all lines of the file
        BufferedReader br = null;
        try
        {
            
            br = new BufferedReader(new FileReader(file));
            // Read the first line
            String line = br.readLine();
            // While there are more valid lines
            while (line != null)
            {
                // Add the line, to the list
                lines.add(line);
                // Get the next line
                line = br.readLine();
            }
        }
        catch (IOException e)
        {
            System.err.println(e);
        }
        finally
        {
            if(br != null)
            {
                try
                {
                    br.close();
                }
                catch (IOException e)
                {
                    System.err.println(e);
                }
            }
        }
        // 4. Write all of these to the response stream.
        PrintWriter out = null;
        try
        {
            out = new PrintWriter(response.getOutputStream());
            for(final String str : lines)
            {
                out.println(str);
            }
        }
        catch (IOException e)
        {
            System.err.println(e);
        }
        finally
        {
            if(out != null)
            {
                out.close();
            }
        }
    }

    public void doGet(HttpServletRequest request, HttpServletResponse response)
    {
        doPost(request, response);
    }
}


