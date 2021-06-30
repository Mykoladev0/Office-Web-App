import React, {Component} from 'react';
import {RouteComponentProps} from 'react-router-dom';
import {Spinner} from '../../../../Shared/ReactComponents/Spinner';
import { IndividualDog } from './IndividualDog';

import { DogModel } from '../../../Models/Dog';
import { getDogById } from '../../../Api/dogService';

type PartialDog = Partial<DogModel>;
const TestDog: PartialDog = {
    gender: "(???)",//unknown from db I think
    dogName: "Happy Go Lucky Dog",
    litterNo: 5654,
    chipNo: "654169",
    birthdate: "2015-01-08T00:00:00",
    abkcNo: "999,999",
    bullyId: 999999,
    id: 0,//new dog
    //following fields are tests to see if they are required
    ownerId:0,
    firstGeneration: true,
    registered: false,
    verified: false,
    printed: false,
    color: "BLUE WHITE",
    breed: "AMERICAN BULLY"
};
const NewDogTemplate: PartialDog = {
    gender: "(???)",//unknown from db I think
    dogName: "",
    // litterNo: 0,
    chipNo: "",
    birthdate: "",
    abkcNo: "",
    //bullyId: 999999,
    id: 0,//new dog
    //following fields are tests to see if they are required
    ownerId:0,
    firstGeneration: true,
    registered: false,
    verified: false,
    printed: false,
    color: "",
    breed: ""
};

export interface ShowDogProps extends RouteComponentProps {
    isReadonly: boolean;
}

export interface ShowDogState {
    isLoading: boolean;
    dog: Partial<DogModel>;
    requestedId: number;
    isNewDog: boolean;
    isReadonly: boolean;
}

//used to retrieve dog information and then choose readonly or edit view
export default class ShowDog extends Component<RouteComponentProps, ShowDogState> {
    // public static defaultProps: Partial<ShowDogProps> = {
    //     isReadonly: true,
    // };

    public state: ShowDogState = {
        isLoading: true,
        dog: null,
        requestedId: -1,
        isNewDog: true,
        isReadonly: true
    };

    public constructor(props) {
        super(props);
    }
    public componentDidMount() {
        const {params} = this.props.match;
        let {isReadonly} = this.state;
        if(params.edit && params.edit === 'edit') {
            isReadonly = false;
        }
        let requestedId: number = -1;
        if(params.id) {
            requestedId = parseInt(params.id, 10);
        }
        //const dog = TestDog;
        // this.setState({
        //     isLoading: false,
        //     dog,
        //     requestedId
        // });
        if(requestedId > 0) {
            getDogById(requestedId).then((dog) => {
                this.setState({
                    isLoading: false,
                    dog,
                    requestedId,
                    isNewDog: false,
                    isReadonly
                });
            });
        } else {
            //new dog!
            this.setState({
                isLoading: false,
                dog: NewDogTemplate,
                // dog: {
                //     id: 0,
                //     gender: 'Unknown',
                // },
                requestedId,
                isNewDog: true,
                isReadonly: false
            });
        }

    }
    public render(): JSX.Element {
        const {isLoading, dog, isNewDog, isReadonly} = this.state;
        const title: string = isNewDog?'Add a new Dog': `Editing ${dog.dogName}, Bully ID: ${dog.bullyId?dog.bullyId:'N/A'}`;
        if(isLoading) {
            return (
            <div className="text-center">
                <Spinner />
            </div>
            );
        }
        if(!dog || dog === null) {
            return(<div>Sorry, the requested dog could not be found.</div>);
        }
        return(
            <IndividualDog
                cardTitle = {title}
                IsEditable = {!isReadonly || isNewDog}
                Dog ={dog}
            />
        );

    }
}
