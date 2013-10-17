import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import java.io.*;

public class AppendWaypoint extends HttpServlet
{
    private static String path = "../webapps/WindowsPhoneServer/data/";

    public void doPost(HttpServletRequest request, HttpServletResponse response)
    {
        // Read all the variables from the url arguments
        String userID    = request.getParameter("userID");
        String routeID   = request.getParameter("routeID");
        String timeStamp = request.getParameter("timeStamp");
        String latitude  = request.getParameter("latitude");
        String longitude = request.getParameter("longitude");
        // Sanity check, that noone of them are null
        if ((userID == null) || (routeID == null) ||
            (timeStamp == null) || (latitude == null) || (longitude == null))
        {
            // TODO: Possibly return a error value, instead of just returning nothing
            System.err.println("NULL ARGUMENTS");
            return;
        }
        // 1. Create a folder with the name UserIDString (if such one doesn’t already exist). 
        String folder_path = path + userID + "/";
        new File(folder_path).mkdirs();
        // 2. Create a file, within that folder, with the name RouteIDString (if such one doesn’t already exist).
        PrintWriter out = null;
        try
        {
            String file_path = folder_path + routeID + ".log";
            out = new PrintWriter(new BufferedWriter(new FileWriter(file_path, true)));
            // 3. Append a line, to the end of that file, with the format;
			//      TimeStampString \t LatitudeString \t LongitudeString
            out.println(timeStamp + "\t" + latitude + "\t" + longitude);
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

