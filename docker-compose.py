import subprocess
import argparse
import time
from threading import Thread

def run_docker_compose(exclude_containers, build, delay_container, delay_seconds):
    base_command = ["docker-compose", "up", "-d"]
    if build:
        base_command.append("--build")
    
    # Read the docker-compose.yml to get the service names
    services_command = ["docker-compose", "config", "--services"]
    result = subprocess.run(services_command, capture_output=True, text=True)
    
    if result.returncode != 0:
        print("Error getting services from docker-compose.yml")
        print(result.stderr)
        return
    
    services = result.stdout.split()
    services_to_run = [service for service in services if service not in exclude_containers]
    
    # Handle the delayed container
    if delay_container and delay_seconds > 0:
        non_delayed_services = [service for service in services_to_run if service != delay_container]
    else:
        non_delayed_services = services_to_run
    
    # Add the services to the command
    final_command = base_command + non_delayed_services
    
    # Run the docker-compose command for the non-delayed services
    if non_delayed_services:
        result = subprocess.run(final_command)
    
        if result.returncode != 0:
            print("Error running docker-compose command")
            print(result.stderr)
    
    if delay_container and delay_seconds > 0:
        def delayed_start():
            print(f"Delaying the start of {delay_container} for {delay_seconds} seconds...")
            time.sleep(delay_seconds)
            delayed_command = ["docker-compose", "up", "-d"]
            if build:
                delayed_command.append("--build")
            delayed_command.append(delay_container)
            result = subprocess.run(delayed_command)
            
            if result.returncode != 0:
                print(f"Error running delayed container {delay_container}")
                print(result.stderr)
            else:
                print(f"{delay_container} started successfully after {delay_seconds} seconds delay.")
        
        thread = Thread(target=delayed_start)
        thread.start()

def main():
    parser = argparse.ArgumentParser(description="Run docker-compose with optional exclusions and delays.")
    parser.add_argument("--exclude", nargs='*', default=[], help="List of containers to exclude.")
    parser.add_argument("--build", action="store_true", help="Add --build flag to docker-compose up.")
    parser.add_argument("--delay", nargs=2, metavar=('CONTAINER', 'SECONDS'), help="Specify a container to delay and the delay time in seconds.")
    
    args = parser.parse_args()
    
    delay_container = None
    delay_seconds = 0
    if args.delay:
        delay_container, delay_seconds = args.delay
        delay_seconds = int(delay_seconds)
    
    run_docker_compose(args.exclude, args.build, delay_container, delay_seconds)

if __name__ == "__main__":
    main()
