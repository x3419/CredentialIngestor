# CredIngestor

Scylla Public Breach Data ELK Pipeline

## How to set up this project:

For this project you will need:
  - Kibana (dockerized)
  - ElasticSearch (dockerized)
  - LogStash (non-dockerized, though this is by preference).

First lets set up elasticsearch:
  - docker run -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" docker.elastic.co/elasticsearch/elasticsearch:7.4.1

To know which elasticsearch container to reference, run the command "docker container ls".
Once we know that, we can initialize kibana:
  - docker run -d -p 5601:5601 -h kibana40 --name kibana --link elated_feistel:elasticsearch kibana:7.4.1
     - In this case, our elasticsearch container is "elated_feistel"

Lastly, we'll fire off LogStash with the config located in this project (logstash.conf):
  - logstash.bat -f C:\path\to\logstash.conf


At this point you should have a working ELK stack. 
If you go to localhost:9200 in your browser you should see that elasticsearch is working.
If you go to localhost:5601 in your browser you should see that kibana is working.
If you go to localhost:5601/status you should see that kibana is successfully connected to elasticsearch
  - NOTE: If at this step you see that elasticsearch is not connected to kibana correctly, the problem is likely your cmdline args

### Scylla API Cloning
Assuming you have now configured everything correctly, you should have LogStash listening on localhost:12345
You can now open this project in Visual Studio, build, and kick it off. It will now begin cloning the Scylla API and feed its results directly into ELK without touching disk. This occurs through a TCP connection between CredIngestor and LogStash.
    