import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import java.io.*;
import java.util.*;

public class GetRoutes extends HttpServlet
{
    private static String path = "../webapps/WindowsPhoneServer/data/";

    public void doPost(HttpServletRequest request, HttpServletResponse response)
    {
        // Read all the variables from the url arguments
        String userID    = request.getParameter("userID");
        // Sanity check, that noone of them are null
        if (userID == null)
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
        // 2. Read the names of all files, within this folder
        // A list to put the resulting strings into
        List<String> files = new ArrayList<String>();
        // Loop all entries in the folder;
        for (final File fileEntry : folder.listFiles())
        {
            // Sub-directories are not allowed
            if (fileEntry.isDirectory())
            {
                System.err.println("DIRECTORIES NOT ALLOWED HERE");
            }
            else
            {
                // Add the name, to the list
                files.add(fileEntry.getName());
                // REMOVE: Debug print it
                System.out.println(fileEntry.getName());
            }
        }
        // 3. Write all of these to the response stream.
        PrintWriter out = null;
        try
        {
            out = new PrintWriter(response.getOutputStream());
            for(final String str : files)
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


