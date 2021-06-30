import React, { Component } from 'react';
import { RouteComponentProps } from 'react-router-dom';
import { Spinner } from '../../../Shared/ReactComponents/Spinner';
import IndividualOwner from './IndividualOwner';

import { OwnerModel } from '../../Models/Owner';
import { getOwnerById } from '../../Api/ownerService';

// const TestDog: PartialDog = {
//   gender: '(???)', //unknown from db I think
//   dogName: 'Happy Go Lucky Dog',
//   litterNo: 5654,
//   chipNo: '654169',
//   birthdate: '2015-01-08T00:00:00',
//   abkcNo: '999,999',
//   bullyId: 999999,
//   id: 0, //new dog
//   //following fields are tests to see if they are required
//   ownerId: 0,
//   firstGeneration: true,
//   registered: false,
//   verified: false,
//   printed: false,
//   color: 'BLUE WHITE',
//   breed: 'AMERICAN BULLY',
// };
const NewTemplate: OwnerModel = {
  firstName: '',
  lastName: '',
  middleInitial: '',
  // id: 0, //new owner
  //following fields are tests to see if they are required
  ownerId: 0, //new owner
  fullName: '',
  address1: '',
  address2: '',
  address3: '',
  city: '',
  state: '',
  zip: '',
  country: '',
  international: false,
  email: '',
  phone: '',
  cell: '',
  dualSignature: false,
  comments: '',
  createDate: '',
  modifyDate: '',
  modifiedBy: '',
};

export interface ShowOwnerProps {
  isReadonly: boolean;
}
type Props = ShowOwnerProps & RouteComponentProps;
export interface ShowOwnerState {
  isLoading: boolean;
  owner: OwnerModel;
  requestedId: number;
  isNew: boolean;
  isReadonly: boolean;
}

//used to retrieve dog information and then choose readonly or edit view
export default class ShowDog extends Component<Props, ShowOwnerState> {
  // public static defaultProps: Partial<ShowDogProps> = {
  //     isReadonly: true,
  // };

  public state: ShowOwnerState = {
    isLoading: true,
    owner: null,
    requestedId: -1,
    isNew: true,
    isReadonly: true,
  };

  public constructor(props) {
    super(props);
  }
  public componentDidMount() {
    const { params } = this.props.match;
    let { isReadonly } = this.state;
    if (params.edit && params.edit === 'edit') {
      isReadonly = false;
    }
    let requestedId: number = -1;
    if (params.id) {
      requestedId = parseInt(params.id, 10);
    }
    //const dog = TestDog;
    // this.setState({
    //     isLoading: false,
    //     dog,
    //     requestedId
    // });
    if (requestedId > 0) {
      getOwnerById(requestedId).then(owner => {
        this.setState({
          isLoading: false,
          owner,
          requestedId,
          isNew: false,
          isReadonly,
        });
      });
    } else {
      //new owner!
      this.setState({
        isLoading: false,
        owner: NewTemplate,
        requestedId,
        isNew: true,
        isReadonly: false,
      });
    }
  }
  public render(): JSX.Element {
    const { isLoading, owner, isNew, isReadonly } = this.state;
    const title: string = isNew ? 'Add a new Owner' : `Editing ${owner.fullName}}`;
    if (isLoading) {
      return (
        <div className="text-center">
          <Spinner />
        </div>
      );
    }
    if (!owner || owner === null) {
      return <div>Sorry, the requested owner could not be found.</div>;
    }
    return <IndividualOwner cardTitle={title} IsEditable={!isReadonly || isNew} Owner={owner} />;
  }
}
