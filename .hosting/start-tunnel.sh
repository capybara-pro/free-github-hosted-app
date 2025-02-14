#!/bin/bash

LOCAL_HOST=${LOCAL_HOST:-app}
LOCAL_PORT=${LOCAL_PORT:-8080}

GITHUB_SERVER_URL=${GITHUB_SERVER_URL:-"github.com"}
GIT_USERNAME=${GIT_USERNAME:-"Tunnel Url Pusher"}
GIT_USER_EMAIL=${GIT_USER_EMAIL:-"tunnel-url-pusher@users.noreply.$GITHUB_SERVER_URL"}
GIT_BRANCH=${GIT_BRANCH:-"tunnel-url"}
[[ -z "$GITHUB_REPOSITORY" ]] && {
  echo >&2 "GITHUB_REPOSITORY is empty"
  exit 1
}
[[ -z "$GITHUB_TOKEN" ]] && {
  echo >&2 "GITHUB_TOKEN is empty"
  exit 1
}

REPOSITORY_PATH="https://x-access-token:$GITHUB_TOKEN@$GITHUB_SERVER_URL/$GITHUB_REPOSITORY.git"

workspace=$(mktemp -d)
cd "$workspace" || exit 1

init_git() {
  git init
  git config --global --add safe.directory "$workspace"
  git config user.name "${GIT_USERNAME}"
  git config user.email "${GIT_USER_EMAIL}"
  git config core.ignorecase false
  git config --local --unset-all "http.https://$GITHUB_SERVER_URL/.extraheader"
  git remote rm origin
  git remote add origin "$REPOSITORY_PATH"
}

commit_public_url() {
  echo "PUBLIC URL is $1"

  if [[ $(git branch --list "$GIT_BRANCH") ]]; then
    git checkout -d "$GIT_BRANCH"
    git branch -D "$GIT_BRANCH"
  fi
  git checkout --orphan "$GIT_BRANCH"
  echo -n "$1" >tunnel-url.txt
  git add tunnel-url.txt
  git commit --no-verify --allow-empty -m "Commit tunnel-url 🚀"
  git push -f origin "$GIT_BRANCH"
}

start_tunnel() {
  echo "Starting tunnel in background on port [ ${LOCAL_PORT} ]"
  npx localtunnel --local-host app --port 8080 |
    tee >(grep -oE --line-buffered '(https:\/\/[[:alnum:]\w-]+\.loca\.lt)' | while IFS= read -r line; do commit_public_url "$line"; done)
}

init_git
start_tunnel
