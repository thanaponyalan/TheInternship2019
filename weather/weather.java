import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.DocumentBuilder;
import org.w3c.dom.Document;
import org.w3c.dom.NodeList;
import org.w3c.dom.Node;
import org.w3c.dom.Element;
import org.w3c.dom.NamedNodeMap;
/**
 * dom
 */
public class weather {
    public static String ret;
    public static void main(String[] args) {
        try{
            File inputFile=new File(args[0]);
            DocumentBuilderFactory dbFactory=DocumentBuilderFactory.newInstance();
            DocumentBuilder dBuilder=dbFactory.newDocumentBuilder();
            Document doc=dBuilder.parse(inputFile);
            doc.getDocumentElement().normalize();
            Element docElement=doc.getDocumentElement();
            ret="{\n";
            xmlToJson(docElement);
            ret+="}";
            // System.out.print(ret);
            try {
                File file = new File("weather.json");
                FileWriter fileWriter = new FileWriter(file);
                fileWriter.write(ret);
                fileWriter.flush();
                fileWriter.close();
                System.out.println("weather.json");
            } catch (IOException e) {
                e.printStackTrace();
            }
        }catch(Exception e){
            e.printStackTrace();
        }
    }
    
    public static boolean isOpen=true;
    public static void xmlToJson(Node node) {
        NamedNodeMap attrList=node.getAttributes();
        NodeList nodeList = node.getChildNodes();
        if(node.getParentNode().getNodeName()!="#document"){
            if(attrList.getLength()==0&&nodeList.getLength()<=1&&nodeList.item(0).getNodeType()==Node.TEXT_NODE){ret+="\""+node.getNodeName()+"\": "+"\""+node.getTextContent()+"\",\n"; isOpen=false;}
            else {ret+="\""+node.getNodeName()+"\": {\n"; isOpen=true;}
        }
        for(int j=0;j<attrList.getLength();j++){
            Node attr=attrList.item(j);
            ret+="\""+attr.getNodeName()+"\" : "+"\""+attr.getNodeValue()+"\"";
            if(j+1<attrList.getLength()||node.hasChildNodes())ret+=",\n";
            else ret+="\n";
        }
        for (int i = 0; i < nodeList.getLength(); i++) {
            Node currentNode = nodeList.item(i);
            if (nodeList.item(i).getNodeType() == Node.ELEMENT_NODE) {
                xmlToJson(currentNode);
                if(isOpen&&i+2<nodeList.getLength())ret+="},\n";
                else if(isOpen)ret+="}\n";
            }
        }
    }
}