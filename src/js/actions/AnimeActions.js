import dispatcher from "../dispatcher";

export function createAnime(text) {
  dispatcher.dispatch({
    type: "CREATE_ANIME",
    text,
  });
}

export function deleteAnime(id) {
  dispatcher.dispatch({
    type: "DELETE_ANIME",
    id,
  });
}

export function reloadAnimes() {
  dispatcher.dispatch({type: "FETCH_ANIMES"});
  fetch('http://192.168.178.70:5000/api/animes')
    .then(result => result.json())
    .then(items => dispatcher.dispatch({
    type: "RECEIVE_ANIMES", animes: items
  }));
}

export function reloadAnimesFollowed(user, token) {
  dispatcher.dispatch({type: "FETCH_ANIMES"});
  fetch('http://192.168.178.70:5000/api/users/0/animes', {
    headers: {'Authorization': 'Bearer ' + localStorage.getItem('id_token') }
  })
    .then(result => result.json())
    .then(items => dispatcher.dispatch({
    type: "RECEIVE_FOLLOWED_ANIMES", animes: items
  }));
}

export function reloadNewEpisodes(user, token) {
  dispatcher.dispatch({type: "FETCH_ANIMES"});
  fetch('http://192.168.178.70:5000/api/users/0/episodes?sortby=anime', {
    headers: {'Authorization': 'Bearer ' + localStorage.getItem('id_token') }
    })
    .then(result => result.json())
    .then(items => {
      dispatcher.dispatch({ type: "RECEIVE_NEW_EPISODES", animes: items });
    });
  
}

export function followAnime(anime, token) {
  fetch('http://192.168.178.70:5000/api/users/0/animes', {
    method: 'POST',
    headers: new Headers({
		  'Content-Type': 'application/json',
      'Authorization': 'Bearer ' + localStorage.getItem('id_token')
	  }),
    body: JSON.stringify({
      id: anime.id
    })
  })
  .then(result => {
    reloadAnimesFollowed();
    dispatcher.dispatch({type: "FOLLOW_ANIME", anime})
    
  })
  .catch(err => {
    reloadAnimesFollowed();
    dispatcher.dispatch({type: "FOLLOW_ANIME", anime});
  });
}

export function unfollowAnime(anime, token) {
  fetch('http://192.168.178.70:5000/api/users/0/animes/' + anime.id, {
    method: 'DELETE',
    headers: {'Authorization': 'Bearer ' + localStorage.getItem('id_token') }
  })
  .then(result => {
    reloadAnimesFollowed();
    dispatcher.dispatch({type: "UNFOLLOW_ANIME", anime});
  })
  .catch(err => {
    reloadAnimesFollowed();
    dispatcher.dispatch({type: "UNFOLLOW_ANIME", anime});
  });
}

export function markAsRead(anime) {
    anime.episodes.forEach(episode => {
      fetch('http://192.168.178.70:5000/api/users/0/episodes/' + episode.id, {
        method: 'DELETE',
        headers: {'Authorization': 'Bearer ' + localStorage.getItem('id_token') }
      })
      .then(result => { 
        console.log(result);
        reloadNewEpisodes();
      })
      .catch(err => { 
        console.log(err);
        reloadNewEpisodes(); 
      });
    });
}