import idb from 'idb';
import { BasicDogLookupInfo } from '../models/dog';
import { ShowParticipantInfo } from '../models/show';
// import cloneDeep from 'lodash/cloneDeep';

const DBVERSION = 1;
const DBNAME = 'ShowAssist';
const SHOWSTABLE = 'shows';
const CLASSTEMPLATETABLE = 'class_templates';
const STYLESTABLE = 'styles';
const BREEDSTABLE = 'breeds';
const DOGSTABLE = 'dogs';
// window.addEventListener('activate', function (event) {
//     //activate event handler of service, remove old cached files. This also makes a good place to initialize IndexedDB database, better than using the service worker’s install event handler, since the old service worker will still be in control at that point, and there could be conflicts if a new database is mixed with an old service worker.
//     event.waitUntil( // event.waitUntil ensures that a service worker does not terminate during asynchronous operations.
//         createDB()
//     );
// });

function createIndexedDB() {
  if (!('indexedDB' in window)) {
    return null;
  }
  return idb.open(DBNAME, DBVERSION, upgradeDb => {
    //do the following for each 'table' we want in the db
    if (!upgradeDb.objectStoreNames.contains(DOGSTABLE)) {
      //const dogsObjectStore =
      upgradeDb.createObjectStore(DOGSTABLE, {
        keyPath: 'id',
      });
    }
    if (!upgradeDb.objectStoreNames.contains(SHOWSTABLE)) {
      upgradeDb.createObjectStore(SHOWSTABLE, {
        keyPath: 'showId',
      });
    }
    if (!upgradeDb.objectStoreNames.contains(CLASSTEMPLATETABLE)) {
      upgradeDb.createObjectStore(CLASSTEMPLATETABLE, {
        keyPath: 'classId',
      });
    }
    if (!upgradeDb.objectStoreNames.contains(STYLESTABLE)) {
      upgradeDb.createObjectStore(STYLESTABLE, {
        keyPath: 'id',
      });
    }
    if (!upgradeDb.objectStoreNames.contains(BREEDSTABLE)) {
      upgradeDb.createObjectStore(BREEDSTABLE, {
        keyPath: 'id',
      });
    }
  });
}

// TODO - create indexedDB database
const dbPromise = createIndexedDB();

const saveShows = (shows: any[]) => {
  return dbPromise.then(db => {
    const tx = db.transaction(SHOWSTABLE, 'readwrite');
    const store = tx.objectStore(SHOWSTABLE);
    return Promise.all(shows.map(s => store.put(s))).catch(err => {
      tx.abort();
      throw Error(`Shows were not added to the store because ${err}.`);
    });
  });
};
///showfilter may return a subset?
const retrieveShows = (showFilter: any = '') => {
  return dbPromise.then(db => {
    const tx = db.transaction(SHOWSTABLE, 'readonly');
    const store = tx.objectStore(SHOWSTABLE);
    return store.getAll(); //apply filter?
  });
};

const retrieveShowById = (showId: number) => {
  return dbPromise.then(db => {
    const tx = db.transaction(SHOWSTABLE, 'readonly');
    const store = tx.objectStore(SHOWSTABLE);
    return store.get(showId);
  });
};

const retrieveClassTemplates = () => {
  return dbPromise.then(db => {
    const tx = db.transaction(CLASSTEMPLATETABLE, 'readonly');
    const store = tx.objectStore(CLASSTEMPLATETABLE);
    return store.getAll(); //apply filter?
  });
};

const saveClassTemplates = (classTemplates: any[]) => {
  return dbPromise.then(db => {
    const tx = db.transaction(CLASSTEMPLATETABLE, 'readwrite');
    const store = tx.objectStore(CLASSTEMPLATETABLE);
    if (!classTemplates || !Array.isArray(classTemplates)) {
      classTemplates = [];
    }
    return Promise.all(classTemplates.map(s => store.put(s))).catch(err => {
      tx.abort();
      throw Error(`Class Templates were not added to the store because ${err}.`);
    });
  });
};
const retrieveStyles = () => {
  return dbPromise.then(db => {
    const tx = db.transaction(STYLESTABLE, 'readonly');
    const store = tx.objectStore(STYLESTABLE);
    return store.getAll(); //apply filter?
  });
};

const saveStyles = (styles: any[]) => {
  return dbPromise.then(db => {
    const tx = db.transaction(STYLESTABLE, 'readwrite');
    const store = tx.objectStore(STYLESTABLE);
    if (!styles || !Array.isArray(styles)) {
      styles = [];
    }
    return Promise.all(styles.map(s => store.put(s))).catch(err => {
      tx.abort();
      throw Error(`Class Templates were not added to the store because ${err}.`);
    });
  });
};

const retrieveBreeds = () => {
  return dbPromise.then(db => {
    const tx = db.transaction(BREEDSTABLE, 'readonly');
    const store = tx.objectStore(BREEDSTABLE);
    return store.getAll(); //apply filter?
  });
};

const saveBreeds = (styles: any[]) => {
  return dbPromise.then(db => {
    const tx = db.transaction(BREEDSTABLE, 'readwrite');
    const store = tx.objectStore(BREEDSTABLE);
    return Promise.all(styles.map(s => store.put(s))).catch(err => {
      tx.abort();
      throw Error(`Breeds were not added to the store because ${err}.`);
    });
  });
};

const retrieveDogs = (): Promise<BasicDogLookupInfo[]> => {
  return dbPromise.then(db => {
    const tx = db.transaction(DOGSTABLE, 'readonly');
    const store = tx.objectStore(DOGSTABLE);
    return store.getAll(); //apply filter?
  });
};

const saveDogs = (dogs: any[]) => {
  return dbPromise.then(db => {
    const tx = db.transaction(DOGSTABLE, 'readwrite');
    const store = tx.objectStore(DOGSTABLE);
    return Promise.all(dogs.map(s => store.put(s))).catch(err => {
      tx.abort();
      throw Error(`Dogs were not added to the store because ${err}.`);
    });
  });
};

const retrieveDogById = (dogId: number) => {
  return dbPromise.then(db => {
    const tx = db.transaction(DOGSTABLE, 'readonly');
    const store = tx.objectStore(DOGSTABLE);
    return store.get(dogId);
  });
};

const saveShowEventsLocally = (
  showId: number,
  classTemplateName: string,
  breed: string,
  gender: string,
  style: string,
  results: EventResult[]
) => {
  return dbPromise.then(db => {
    const tx = db.transaction(SHOWSTABLE, 'readwrite');
    const store = tx.objectStore(SHOWSTABLE);
    return Promise.all(
      results.map(result => {
        //get s
        store.get(showId).then(show => {
          if (!show.events || !Array.isArray(show.events)) {
            show.events = [];
          }
          //find event
          // if (style && style !== '') {
          //   console.log('style set, need to use this!');
          // }
          let evt = show.events.find(
            e =>
              e.breed === breed &&
              e.class === classTemplateName &&
              e.gender === gender &&
              e.style === style
          ); //add style check
          if (!evt) {
            evt = {
              breed,
              class: classTemplateName,
              gender,
              style,
              results: [],
            };
            show.events.push(evt);
          }
          //now add/update result
          const resultIndex = evt.results.findIndex(r => r.id === result.id);
          if (resultIndex > -1) {
            evt.results.splice(resultIndex, 1, result); //replace with new one
          } else {
            evt.results.push(result);
          }
          store.put(show);
        });
      })
    ).catch(err => {
      tx.abort();
      throw Error(`Events were not added to the store because ${err}.`);
    });
  });
};

const saveParticipantsWithDogInfo = (participants: ShowParticipantInfo[]) => {
  //break out dog info, save that to dogs table
  return Promise.all(
    participants.map(p => {
      // const participant = {
      //   id: p.participantId,
      //   dogId: p['id'],
      //   showId: p.showId,
      //   dateRegistered: p.dateRegistered,
      // };
      // let dog = cloneDeep(p);
      // const { participantId, showId, dateRegistered, ...dog } = cloneDeep(p);
      //then register with show
      return Promise.all([registerDogLocally(p), saveDogs([p.dog])]);
    })
  );
};

const registerDogLocally = (registration: any) => {
  /*
  {
  id: 0, //placeholder
  dogId: dog.id,
  showId,
  armbandNumber,
};
  */
  const { showId } = registration;
  return dbPromise.then(db => {
    const tx = db.transaction(SHOWSTABLE, 'readwrite');
    const store = tx.objectStore(SHOWSTABLE);
    return Promise.all(
      [registration].map(participant => {
        //get s
        store.get(showId).then(show => {
          if (!show.participants || !Array.isArray(show.participants)) {
            show.participants = [];
          }

          const pIndex = show.participants.findIndex(p => {
            if (p.id && participant.id && p.id === participant.id && participant.id !== 0) {
              return true;
            }
            return p.dogId === participant.dogId;
          });
          const partToPush = {
            showId: participant.showId,
            id: participant.id,
            dateRegistered: participant.dateRegistered,
            dogId: participant.dog!==null && participant.dog !== undefined ?participant.dog.id:participant.dogId,
            armbandNumber: participant.armbandNumber,
          };
          if (pIndex === -1) {
            show.participants.push(partToPush);
          } else {
            //update participant from outside, must be updated!
            show.participants.splice(pIndex, 1, partToPush);
          }
          store.put(show);
        });
      })
    ).catch(err => {
      tx.abort();
      throw Error(`Registration ${registration} was not added to the store because ${err}.`);
    });
  });
};

async function getShowParticipants(showId: number): Promise<ShowParticipantInfo[]> {
  //get show
  const show = await retrieveShowById(showId);
  let participants = show.participants;
  if (!participants || !Array.isArray(participants)) {
    participants = [];
  }
  //hydrate participants
  for (const p of participants) {
    p.dog = await retrieveDogById(p.dogId);
  }
  return participants;
}

export {
  dbPromise,
  saveShows,
  retrieveShows,
  retrieveShowById,
  retrieveClassTemplates,
  saveClassTemplates,
  retrieveStyles,
  saveStyles,
  retrieveBreeds,
  saveBreeds,
  retrieveDogById,
  retrieveDogs,
  saveDogs,
  saveShowEventsLocally,
  registerDogLocally,
  saveParticipantsWithDogInfo,
  getShowParticipants,
};
