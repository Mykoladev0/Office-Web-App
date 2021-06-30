import React, { Component } from 'react';
import { Route, Switch } from 'react-router-dom';

// import { DogsTable } from './DogsTable';
import ShowDog from './IndividualDog/ShowDog';
import DogsSearch from './DogsSearch';

const DogsViewFunc = props => {
  const { match } = props;

  return (
    <div>
      <Switch>
        {/* <Route exact path={`${match.url}`} component={DogsTable} /> */}
        <Route
          exact
          path={`${match.url}`}
          render={() => {
            return <DogsSearch {...props} />;
          }}
        />
        <Route exact path={`${match.url}/new`} params component={ShowDog} /> {/*new dog */}
        <Route exact path={`${match.url}/:id/:edit`} params component={ShowDog} /> {/* new */}
        <Route path={`${match.url}/:id`} params component={ShowDog} /> {/*readonly view*/}
        {/* <Redirect to={{
            state: { error: true }
            }} /> */}
      </Switch>
      {/* <Route path={`${match.url}/:topicId`} component={Topic}/> */}
    </div>
  );
};

export { DogsViewFunc };
