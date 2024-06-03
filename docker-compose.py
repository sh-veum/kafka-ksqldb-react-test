import subprocess
import argparse

def run_docker_compose(exclude_containers, build):
    base_command = ["docker-compose", "up"]
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
    
    # Add the services to the command
    final_command = base_command + services_to_run
    
    # Run the docker-compose command
    result = subprocess.run(final_command)
    
    if result.returncode != 0:
        print("Error running docker-compose command")
        print(result.stderr)

def main():
    parser = argparse.ArgumentParser(description="Run docker-compose with optional exclusions.")
    parser.add_argument("--exclude", nargs='*', default=[], help="List of containers to exclude.")
    parser.add_argument("--build", action="store_true", help="Add --build flag to docker-compose up.")
    
    args = parser.parse_args()
    
    run_docker_compose(args.exclude, args.build)

if __name__ == "__main__":
    main()
