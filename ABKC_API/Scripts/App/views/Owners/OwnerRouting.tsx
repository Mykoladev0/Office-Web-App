import React from 'react';
import { Route, Switch } from 'react-router-dom';

// import { DogsTable } from './DogsTable';
import ShowOwner from './ShowOwner';
import OwnerSearch from './OwnerSearch';

const OwnersViewFunc = props => {
  const { match } = props;

  return (
    <div>
      <Switch>
        {/* <Route exact path={`${match.url}`} component={DogsTable} /> */}
        <Route
          exact
          path={`${match.url}`}
          render={() => {
            return <OwnerSearch {...props} />;
          }}
        />
        <Route exact path={`${match.url}/new`} params component={ShowOwner} /> {/*new owner */}
        <Route exact path={`${match.url}/:id/:edit`} params component={ShowOwner} /> {/* new */}
        <Route path={`${match.url}/:id`} params component={ShowOwner} /> {/*readonly view*/}
        {/* <Redirect to={{
            state: { error: true }
            }} /> */}
      </Switch>
      {/* <Route path={`${match.url}/:topicId`} component={Topic}/> */}
    </div>
  );
};

export { OwnersViewFunc };
