import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import java.io.*;

public class LoginServlet extends HttpServlet
{
    private PrintWriter out = null;
    public LoginServlet()
    {
        try
        {
            out = new PrintWriter(new BufferedWriter(new FileWriter("../webapps/WindowsPhoneServer/output.log")));
        }
        catch(Exception e)
        {
        }
    }

    public void doPost(HttpServletRequest request, HttpServletResponse response)
    {
        String latitude  = request.getParameter("Latitude");
        String longitude = request.getParameter("Longitude");

        out.print(latitude);
        out.print("\t");
        out.print(longitude);
        out.print("\n");
        out.flush();
    }
    
    public void doGet(HttpServletRequest request, HttpServletResponse response)
    {
        doPost(request, response);
    }
}

